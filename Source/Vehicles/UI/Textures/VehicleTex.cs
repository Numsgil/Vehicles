﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using SmashTools;

namespace Vehicles
{
	[StaticConstructorOnStartup]
	public static class VehicleTex
	{
		public static readonly Texture2D UnloadAll = ContentFinder<Texture2D>.Get("UI/Gizmos/UnloadAll");

		public static readonly Texture2D UnloadPassenger = ContentFinder<Texture2D>.Get("UI/Gizmos/UnloadPawn");

		public static readonly Texture2D Anchor = ContentFinder<Texture2D>.Get("UI/Gizmos/Anchor");

		public static readonly Texture2D Rename = ContentFinder<Texture2D>.Get("UI/Buttons/Rename");

		public static readonly Texture2D Recolor = ContentFinder<Texture2D>.Get("UI/ColorTools/Paintbrush");

		public static readonly Texture2D Drop = ContentFinder<Texture2D>.Get("UI/Buttons/Drop");

		public static readonly Texture2D FishingIcon = ContentFinder<Texture2D>.Get("UI/Gizmos/FishingGizmo");

		public static readonly Texture2D CaravanIcon = ContentFinder<Texture2D>.Get("UI/Commands/FormCaravan");

		public static readonly Texture2D PackCargoIcon = ContentFinder<Texture2D>.Get("UI/Gizmos/StartLoad");

		public static readonly Texture2D CancelPackCargoIcon = ContentFinder<Texture2D>.Get("UI/Gizmos/CancelLoad");

		public static readonly Texture2D AmmoBG = ContentFinder<Texture2D>.Get("UI/Gizmos/AmmoBoxBG");

		public static readonly Texture2D ReloadIcon = ContentFinder<Texture2D>.Get("UI/Gizmos/Reload");

		public static readonly Texture2D AutoTargetIcon = ContentFinder<Texture2D>.Get("UI/Gizmos/AutoTarget");

		public static readonly Texture2D HaltIcon = ContentFinder<Texture2D>.Get("UI/Commands/Halt");

		public static readonly List<Texture2D> FireIcons = ContentFinder<Texture2D>.GetAllInFolder("Things/Special/Fire").ToList();

		public static readonly Texture2D FullBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.35f, 0.35f, 0.2f));

		public static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(Color.black);

		public static readonly Texture2D TargetLevelArrow = ContentFinder<Texture2D>.Get("UI/Misc/BarInstantMarkerRotated");

		public static readonly Texture2D SwitchLeft = ContentFinder<Texture2D>.Get("UI/ColorTools/SwitchLeft");

		public static readonly Texture2D SwitchRight = ContentFinder<Texture2D>.Get("UI/ColorTools/SwitchRight");

		public static readonly Texture2D ReverseIcon = ContentFinder<Texture2D>.Get("UI/ColorTools/SwapColors");

		public static readonly Texture2D FlickerIcon = ContentFinder<Texture2D>.Get("UI/Commands/DesirePower");

		public static readonly Texture2D DismissTex = ContentFinder<Texture2D>.Get("UI/Commands/DismissShuttle");
		
		public static readonly Texture2D TargeterMouseAttachment = ContentFinder<Texture2D>.Get("UI/Overlays/LaunchableMouseAttachment");
		
		public static readonly Texture2D LaunchCommandTex = ContentFinder<Texture2D>.Get("UI/Commands/LaunchShip");

		public static readonly Texture2D ResetPage = ContentFinder<Texture2D>.Get("UI/Settings/ResetPage");

		public static readonly Texture2D ResetAll = ContentFinder<Texture2D>.Get("UI/Settings/ResetAll");

		public static readonly Texture2D TradeCommandTex = ContentFinder<Texture2D>.Get("UI/Commands/Trade");

		public static readonly Texture2D OfferGiftsCommandTex = ContentFinder<Texture2D>.Get("UI/Commands/OfferGifts");

		public static readonly Texture2D AltitudeMeter = ContentFinder<Texture2D>.Get("UI/Gizmos/AltitudeMeter");

		public static readonly Texture2D TradeArrow = ContentFinder<Texture2D>.Get("UI/Widgets/TradeArrow");

		public static readonly Texture2D ColorPicker = ContentFinder<Texture2D>.Get("UI/ColorTools/ColorCog");

		public static readonly Texture2D ColorHue = ContentFinder<Texture2D>.Get("UI/ColorTools/ColorHue");

		public static readonly Material LandingTargeterMat = MaterialPool.MatFrom("UI/Icons/LandingTargeter", ShaderDatabase.Transparent);

		public static readonly Material RangeCircle_ExtraWide = MaterialPool.MatFrom("UI/RangeField_ExtraWide", ShaderDatabase.MoteGlow);

		public static readonly Material RangeCircle_Wide = MaterialPool.MatFrom("UI/RangeField_Wide", ShaderDatabase.MoteGlow);

		public static readonly Material RangeCircle_Mid = MaterialPool.MatFrom("UI/RangeField_Mid", ShaderDatabase.MoteGlow);

		public static readonly Material RangeCircle_Close = MaterialPool.MatFrom("UI/RangeField_Close", ShaderDatabase.MoteGlow);

		public static readonly Dictionary<VehicleDef, Texture2D> CachedTextureIcons = new Dictionary<VehicleDef, Texture2D>();

		public static readonly Dictionary<Pair<VehicleDef, Rot8>, Texture2D> CachedVehicleTextures = new Dictionary<Pair<VehicleDef, Rot8>, Texture2D>();

		public static readonly Dictionary<VehicleDef, Graphic_Vehicle> CachedGraphics = new Dictionary<VehicleDef, Graphic_Vehicle>();

		private static readonly Dictionary<string, Texture2D> cachedTextureFilepaths = new Dictionary<string, Texture2D>();

		#if BETA
		internal static readonly Texture2D BetaButtonIcon = ContentFinder<Texture2D>.Get("Beta/BetaButton");

		internal static readonly Texture2D DiscordIcon = ContentFinder<Texture2D>.Get("Beta/discordIcon");

		internal static readonly Texture2D GithubIcon = ContentFinder<Texture2D>.Get("Beta/githubIcon");

		internal static readonly Texture2D SteamIcon = ContentFinder<Texture2D>.Get("Beta/steamIcon");
		#endif

		static VehicleTex()
		{
			foreach (VehicleDef vehicleDef in DefDatabase<VehicleDef>.AllDefs)
			{
				string iconFilePath = vehicleDef.properties.iconTexPath;
				if (iconFilePath.NullOrEmpty())
				{
					switch (vehicleDef.vehicleType)
					{
						case VehicleType.Land:
							iconFilePath = "UI/Icons/DefaultVehicleIcon";
							break;
						case VehicleType.Sea:
							iconFilePath = "UI/Icons/DefaultBoatIcon";
							break;
						case VehicleType.Air:
							iconFilePath = "UI/Icons/DefaultPlaneIcon";
							break;
					}
				}
				if (vehicleDef.race.AnyPawnKind.lifeStages.Last().bodyGraphicData is GraphicDataRGB bodyGraphicData)
				{
					Texture2D tex;
					var graphicData = new GraphicDataRGB();
					graphicData.CopyFrom(bodyGraphicData);
					Graphic_Vehicle graphic = graphicData.Graphic as Graphic_Vehicle;
					SetTextureCache(vehicleDef, graphicData);
					if (cachedTextureFilepaths.ContainsKey(iconFilePath))
					{
						tex = cachedTextureFilepaths[iconFilePath];
					}
					else
					{
						tex = ContentFinder<Texture2D>.Get(iconFilePath);
						cachedTextureFilepaths.Add(iconFilePath, tex);
					}
					CachedGraphics.Add(vehicleDef, graphic);
					CachedTextureIcons.Add(vehicleDef, tex);
				}
				else
				{
					SmashLog.Error($"Must use <type>GraphicDataRGB</type> for <type>VehicleDef</type>.");
				}
			}
		}

		public static Texture2D VehicleTexture(VehicleDef def, Rot8 rot)
		{
			return CachedVehicleTextures.TryGetValue(new Pair<VehicleDef, Rot8>(def, rot), CachedVehicleTextures[new Pair<VehicleDef, Rot8>(def, Rot8.North)]);
		}

		private static void SetTextureCache(VehicleDef vehicleDef, GraphicDataRGB graphicData)
		{
			var textureArray = new Texture2D[Graphic_RGB.MatCount];
			textureArray[0] = ContentFinder<Texture2D>.Get(graphicData.texPath + "_north", false);
			textureArray[1] = ContentFinder<Texture2D>.Get(graphicData.texPath + "_east", false);
			textureArray[2] = ContentFinder<Texture2D>.Get(graphicData.texPath + "_south", false);
			textureArray[3] = ContentFinder<Texture2D>.Get(graphicData.texPath + "_west", false);
			textureArray[4] = ContentFinder<Texture2D>.Get(graphicData.texPath + "_northEast", false);
			textureArray[5] = ContentFinder<Texture2D>.Get(graphicData.texPath + "_southEast", false);
			textureArray[6] = ContentFinder<Texture2D>.Get(graphicData.texPath + "_southWest", false);
			textureArray[7] = ContentFinder<Texture2D>.Get(graphicData.texPath + "_northWest", false);
			
			if (textureArray[0] == null)
			{
				if (textureArray[2] != null)
				{
					textureArray[0] = textureArray[2];
				}
				else if (textureArray[1] != null)
				{
					textureArray[0] = textureArray[1];
				}
				else if (textureArray[3] != null)
				{
					textureArray[0] = textureArray[3];
				}
				else
				{
					textureArray[0] = ContentFinder<Texture2D>.Get(graphicData.texPath, false);
				}
			}
			if (textureArray[0] == null)
			{
				Log.Error($"Failed to find any textures at {graphicData.texPath} while constructing texture cache.");
				return;
			}
			if (textureArray[2] == null)
			{
				textureArray[2] = textureArray[0];
			}
			if (textureArray[1] == null)
			{
				if (textureArray[3] != null)
				{
					textureArray[1] = textureArray[3];
				}
				else
				{
					textureArray[1] = textureArray[0];
				}
			}
			if (textureArray[3] == null)
			{
				if (textureArray[1] != null)
				{
					textureArray[3] = textureArray[1];
				}
				else
				{
					textureArray[3] = textureArray[0];
				}
			}

			if(textureArray[5] == null)
			{
				if(textureArray[4] != null)
				{
					textureArray[5] = textureArray[4];
				}
				else
				{
					textureArray[5] = textureArray[1];
				}
			}
			if(textureArray[6] == null)
			{
				if(textureArray[7] != null)
				{
					textureArray[6] = textureArray[7];
				}
				else
				{
					textureArray[6] = textureArray[3];
				}
			}
			if(textureArray[4] == null)
			{
				if(textureArray[5] != null)
				{
					textureArray[4] = textureArray[5];
				}
				else
				{
					textureArray[4] = textureArray[1];
				}
			}
			if(textureArray[7] == null)
			{
				if(textureArray[6] != null)
				{
					textureArray[7] = textureArray[6];
				}
				else
				{
					textureArray[7] = textureArray[3];
				}
			}

			for (int i = 0; i < 8; i++)
			{
				CachedVehicleTextures.Add(new Pair<VehicleDef, Rot8>(vehicleDef, new Rot8(i)), textureArray[i]);
			}
		}
	}
}