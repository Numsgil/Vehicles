﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Vehicles
{
    public class CannonDef : Def
    {
        public WeaponType weaponType;
        public WeaponLocation weaponLocation;
        public ThingDef projectile;
        public ThingDef moteCannon;
        public ThingDef moteFlash;

        public List<ThingDef> ammoAllowed = new List<ThingDef>();
        public int magazineCapacity = 1;
        public bool genericAmmo = false;

        public SoundDef cannonSound;
        public SoundDef reloadSound;

        public GraphicData graphicData;

        public string baseCannonTexPath;

        public string gizmoDescription;
        public string gizmoIconTexPath;
        public float gizmoIconScale = 1f;

        public List<FireMode> fireModes = new List<FireMode>();

        public List<float> centerPoints = new List<float>();
        public List<int> cannonsPerPoint = new List<int>();

        public List<float> projectileShifting = new List<float>();

        public ProjectileHitFlags hitFlags = ProjectileHitFlags.All;

        public bool splitCannonGroups = false;

        public bool autoSnapTargeting = true;

        public bool matchParentColor = true;

        public float rotationSpeed = 1f;

        public float spreadRadius = 0f;

        public float maxRange = -1f;

        public float minRange = 0f;

        public float reloadTimer = 5;

        public float warmUpTimer = 3;

        public int numberCannons = 1;

        public float spacing = 0f;

        public float offset = 0f;

        public float projectileOffset = 0f;

        public float moteSpeedThrown = 2;
    }
}