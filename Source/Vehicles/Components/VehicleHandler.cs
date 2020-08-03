﻿using RimWorld.Planet;
using Verse;
using Vehicles.Defs;
using System.Collections.Generic;

namespace Vehicles
{
    public class VehicleHandler : IExposable, ILoadReferenceable, IThingHolder
    {
        public ThingOwner<Pawn> handlers;

        public VehicleRole role;

        public List<Pawn> currentlyReserving = new List<Pawn>();

        private List<Pawn> tempSavedPawns = new List<Pawn>();

        public int uniqueID = -1;
        public Pawn vehiclePawn;
        
        public VehicleHandler()
        {
            if(handlers is null)
            {
                handlers = new ThingOwner<Pawn>(this, false, LookMode.Reference);
            }
        }

        public VehicleHandler(Pawn vehiclePawn)
        {
            uniqueID = Current.Game.GetComponent<VehicleIdManager>().GetNextHandlerId();
            this.vehiclePawn = vehiclePawn;
            if(handlers is null)
            {
                handlers = new ThingOwner<Pawn>(this, false, LookMode.Reference);
            }
        }

        public VehicleHandler(Pawn vehiclePawn, VehicleRole newRole)
        {
            List<Pawn> newHandlers = new List<Pawn>();
            uniqueID = Current.Game.GetComponent<VehicleIdManager>().GetNextHandlerId();
            this.vehiclePawn = vehiclePawn;
            role = new VehicleRole(newRole);
            if (handlers is null)
            {
                handlers = new ThingOwner<Pawn>(this, false, LookMode.Reference);
            }
            if(currentlyReserving is null)
            {
                currentlyReserving = new List<Pawn>();
            }
            if((newHandlers?.Count ?? 0) > 0)
            {
                foreach(Pawn p in newHandlers)
                {
                    if(p.Spawned) { p.DeSpawn(); }
                    if(p.holdingOwner != null) { p.holdingOwner = null; }
                    if (!p.IsWorldPawn()) { Find.WorldPawns.PassToWorld(p, PawnDiscardDecideMode.Decide); }
                }
                handlers.TryAddRangeOrTransfer(newHandlers);
            }
        }

        public void ReservationHandler()
        {
            if (currentlyReserving is null) currentlyReserving = new List<Pawn>();

            currentlyReserving.RemoveDuplicates();
            for(int i = 0; i < currentlyReserving.Count; i++)
            {
                Pawn p = currentlyReserving[i];
                if (!p.Spawned || p.InMentalState || p.Downed || p.Dead || (p.CurJob.def != JobDefOf_Vehicles.Board && (p.CurJob.targetA.Thing as Pawn) != vehiclePawn))
                {
                    currentlyReserving.Remove(p);
                }
            }
        }

        public bool AreSlotsAvailable
        {
            get
            { 
                return role != null && ((this?.handlers?.Count ?? 0) + (currentlyReserving?.Count ?? 0)) >= role.slots ? false : true;
            }
        }

        public static bool operator ==(VehicleHandler obj1, VehicleHandler obj2) => obj1.Equals(obj2);

        public static bool operator !=(VehicleHandler obj1, VehicleHandler obj2) => !(obj1 == obj2);

        public override bool Equals(object obj)
        {
            return Equals((VehicleHandler)obj);
        }

        public bool Equals(VehicleHandler obj2)
        {
            return obj2?.role.key == role.key;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref uniqueID, "uniqueID", -1);
            Scribe_References.Look(ref vehiclePawn, "vehiclePawn");
            Scribe_Deep.Look(ref role, "role");

            if (Scribe.mode == LoadSaveMode.Saving)
            {
                tempSavedPawns.Clear();
                tempSavedPawns.AddRange(handlers.InnerListForReading);
                handlers.RemoveAll(x => x is Pawn);
                this.handlers.RemoveAll(x => x.Destroyed);
            }

            Scribe_Collections.Look(ref tempSavedPawns, "tempSavedPawns", LookMode.Reference);
            Scribe_Collections.Look(ref currentlyReserving, "currentlyReserving", LookMode.Deep);
            Scribe_Deep.Look<ThingOwner<Pawn>>(ref handlers, "handlers", new object[]
            {
                this
            });

            if (Scribe.mode == LoadSaveMode.PostLoadInit || Scribe.mode == LoadSaveMode.Saving)
            {
                for (int j = 0; j < tempSavedPawns.Count; j++)
                {
                    handlers.TryAdd(tempSavedPawns[j], true);
                }
                tempSavedPawns.Clear();
            }
        }

        public string GetUniqueLoadID()
        {
            return $"VehicleHandler_{uniqueID}";
        }

        public IThingHolder ParentHolder => vehiclePawn;

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return handlers;
        }
    }
}