﻿namespace NitrogenMod.Items
{
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Handlers;
    using System;
    using UnityEngine;

    internal abstract class DummySuitItems : Spawnable
    {
        public static TechType RiverEelScaleID { get; protected set; }
        public static TechType LavaLizardScaleID { get; protected set; }
        public static TechType ThermoBacteriaID { get; protected set; }

        internal static void PatchDummyItems()
        {
            var riverEelScale = new RiverEelScale();
            var lavaLizardScale = new LavaLizardScale();
            var thermoSample = new ThermophileSample();

            riverEelScale.Patch();
            lavaLizardScale.Patch();
            thermoSample.Patch();

                //Todo: These need to be updated to BZ Creatures.
                CraftDataHandler.SetHarvestOutput(TechType.SquidShark, RiverEelScaleID);
                CraftDataHandler.SetHarvestType(TechType.SquidShark, HarvestType.DamageAlive);

                CraftDataHandler.SetHarvestOutput(TechType.RockPuncher, ThermoBacteriaID);
                CraftDataHandler.SetHarvestType(TechType.RockPuncher, HarvestType.DamageAlive);

                CraftDataHandler.SetHarvestOutput(TechType.Cryptosuchus, LavaLizardScaleID);
                CraftDataHandler.SetHarvestType(TechType.Cryptosuchus, HarvestType.DamageAlive);
        }

        protected abstract TechType BaseType { get; }

        protected DummySuitItems(string classID, string friendlyName, string description) : base(classID, friendlyName, description)
        {
            
        }

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(this.BaseType);
            var obj = GameObject.Instantiate(prefab);

            return obj;
        }
    }

    class RiverEelScale : DummySuitItems
    {
        public RiverEelScale()
            : base(classID: "rivereelscale", friendlyName: "River Prowler Scale", description: "A scale from the head of a River Prowler. Has uses in depth-resistant fabrication.")
        {
            OnFinishedPatching += SetStaticTechType;
        }

        protected override TechType BaseType { get; } = TechType.StalkerTooth;

        public override string AssetsFolder { get; } = @"BZNitrogenMod/Assets";

        private void SetStaticTechType() => RiverEelScaleID = this.TechType;
    }

    class LavaLizardScale : DummySuitItems
    {
        public LavaLizardScale()
            : base(classID: "lavalizardscale", friendlyName: "Lava Lizard Scale", description: "A scale from a Lava Lizard. Has uses in depth and heat resistant fabrication.")
        {
            OnFinishedPatching += SetStaticTechType;
        }

        protected override TechType BaseType { get; } = TechType.StalkerTooth;

        public override string AssetsFolder { get; } = @"BZNitrogenMod/Assets";

        private void SetStaticTechType() => LavaLizardScaleID = this.TechType;
    }

    class ThermophileSample : DummySuitItems
    {
        public ThermophileSample()
            : base(classID: "thermophilesample", friendlyName: "Thermophile Bacterial Sample", description: "A viable sample of a unique thermophile bacteria found in Lava Larvae. Undergoes chemosynthesis at high temperatures.")
        {
            OnFinishedPatching += SetStaticTechType;
        }

        protected override TechType BaseType { get; } = TechType.StalkerTooth;

        public override string AssetsFolder { get; } = @"BZNitrogenMod/Assets";

        private void SetStaticTechType() => ThermoBacteriaID = this.TechType;
    }
}