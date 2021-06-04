﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;
using RimWorld.Planet;
using SmashTools;

namespace Vehicles
{
	public class AirDefensePositionTracker : WorldComponent 
	{
		public const float RotationRate = 0.35f;

		private static Dictionary<AerialVehicleInFlight, HashSet<AirDefense>> searchingDefenses = new Dictionary<AerialVehicleInFlight, HashSet<AirDefense>>();
		private static HashSet<AirDefense> defensesToDraw = new HashSet<AirDefense>();

		public static Dictionary<WorldObject, AirDefense> airDefenseCache = new Dictionary<WorldObject, AirDefense>();
		//private Dictionary<WorldObject, AirDefense> airDefenseCacheSaveable = new Dictionary<WorldObject, AirDefense>();
		
		public AirDefensePositionTracker(World world) : base(world)
		{
		}

		public static AirDefensePositionTracker Instance { get; private set; }

		public override void WorldComponentUpdate()
		{
			foreach (AirDefense airDefense in defensesToDraw)
			{
				airDefense.DrawSpotlightOverlay();
			}
		}

		public override void WorldComponentTick()
		{
			foreach (var defense in searchingDefenses)
			{
				AerialVehicleInFlight aerialVehicleSearchingFor = defense.Key;
				for (int j = defense.Value.Count - 1; j >= 0; j--)
				{
					AirDefense airDefense = defense.Value.ElementAt(j);
					float distance = Ext_Math.SphericalDistance(airDefense.parent.DrawPos, aerialVehicleSearchingFor.DrawPos);
					bool withinMaxDistance = distance <= airDefense.MaxDistance;
					if (airDefense.CurrentTarget != aerialVehicleSearchingFor)
					{
						airDefense.angle = (airDefense.angle + RotationRate * airDefense.searchDirection).ClampAndWrap(0, 360);
						float angleToTarget = airDefense.parent.DrawPos.AngleToPoint(aerialVehicleSearchingFor.DrawPos);
						if (withinMaxDistance && Mathf.Abs(angleToTarget - airDefense.angle) <= airDefense.Arc)
						{
							airDefense.activeTargets.Add(aerialVehicleSearchingFor);
						}
					}
					else
					{
						float headingToTarget = WorldHelper.TryFindHeading(airDefense.parent.DrawPos, airDefense.CurrentTarget.DrawPos);
						int dirSign = headingToTarget < airDefense.angle ? -1 : 1;
						if (Mathf.Abs(headingToTarget - airDefense.angle) < 1)
						{
							airDefense.angle = headingToTarget;
						}
						else
						{
							airDefense.angle = (airDefense.angle + RotationRate * dirSign).ClampAndWrap(0, 360);
						}
						if (!withinMaxDistance)
						{
							airDefense.activeTargets.Remove(aerialVehicleSearchingFor);
						}
					}
				}
			}
		}

		public override void FinalizeInit()
		{
			searchingDefenses ??= new Dictionary<AerialVehicleInFlight, HashSet<AirDefense>>();
			airDefenseCache ??= new Dictionary<WorldObject, AirDefense>();
		}

		public static void RegisterAerialVehicle(AerialVehicleInFlight aerialVehicle, HashSet<AirDefense> newDefenses)
		{
			HashSet<AirDefense> oldDefenses = new HashSet<AirDefense>();
			if (searchingDefenses.TryGetValue(aerialVehicle, out var defenses))
			{
				oldDefenses.AddRange(defenses);
				defenses.Clear();
				defenses.AddRange(newDefenses);
			}
			else
			{
				searchingDefenses.Add(aerialVehicle, newDefenses);
			}
			defensesToDraw.RemoveWhere(d => oldDefenses.Contains(d));
			foreach (AirDefense airDefense in newDefenses)
			{
				if (defensesToDraw.Add(airDefense))
				{
					airDefense.angle = Rand.RangeInclusive(0, 360);
				}
			}
		}

		public static void DeregisterAerialVehicle(AerialVehicleInFlight aerialVehicle)
		{
			searchingDefenses.Remove(aerialVehicle);
			RecacheAirDefenseDrawers();
		}

		public static void RecacheAirDefenseDrawers()
		{
			defensesToDraw.Clear();
			foreach (var registeredDefenses in searchingDefenses)
			{
				foreach (AirDefense airDefense in registeredDefenses.Value)
				{
					defensesToDraw.Add(airDefense);
				}
			}
		}

		public static void HighlightEnemySettlements()
		{
			if (WorldRendererUtility.WorldRenderedNow)
			{
				//foreach (AirDefense airDefense in activeSettlementDefenses)
				//{
				//	GenDraw.DrawWorldRadiusRing(airDefense.parent.Tile, Mathf.CeilToInt(airDefense.antiAircraft.properties.distance));
				//}
			}
		}

		public static IEnumerable<AirDefense> CheckNearbyObjects(AerialVehicleInFlight aerialVehicle, float speedPctPerTick)
		{
			float halfTicksPerTileTraveled = Ext_Math.RoundTo(speedPctPerTick * 100, 0.001f);
			Vector3 start = aerialVehicle.DrawPos;
			for (int i = 0; i < aerialVehicle.flightPath.Path.Count; i++)
			{
				int destination = aerialVehicle.flightPath[i].tile;
				Vector3 destinationPos = Find.WorldGrid.GetTileCenter(destination);
				Vector3 position = start;
				for (float transition = 0; transition < 1; transition += halfTicksPerTileTraveled)
				{
					Vector3 partition = Vector3.Slerp(position, destinationPos, transition);
					foreach (KeyValuePair<WorldObject, AirDefense> defenseCache in airDefenseCache)
					{
						float distance = Ext_Math.SphericalDistance(partition, defenseCache.Key.DrawPos);
						if (distance < defenseCache.Value.MaxDistance)
						{
							yield return defenseCache.Value;
						}
					}
				}
				start = destinationPos;
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref airDefenseCache, "airDefenseCache", LookMode.Reference);
		}
	}
}