namespace NitrogenMod.Items
{
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Handlers;
    using System;
    using UnityEngine;

    internal abstract class DummySuitItems : Spawnable
    {
        public static TechType SquidSharkScaleID { get; protected set; }
        public static TechType CryptosuchusScaleID { get; protected set; }
        public static TechType ThermoBacteriaID { get; protected set; }


        internal static void PatchDummyItems()
        {
            var squidSharkScale = new SquidSharkScale();
            var cryptosuchusScale = new CryptosuchusScale();
            var thermoSample = new ThermophileSample();

            squidSharkScale.Patch();
            cryptosuchusScale.Patch();
            thermoSample.Patch();

            CraftDataHandler.SetHarvestOutput(TechType.SquidShark, SquidSharkScaleID);
            CraftDataHandler.SetHarvestType(TechType.SquidShark, HarvestType.DamageAlive);

            CraftDataHandler.SetHarvestOutput(TechType.RockPuncher, ThermoBacteriaID);
            CraftDataHandler.SetHarvestType(TechType.RockPuncher, HarvestType.DamageAlive);

            CraftDataHandler.SetHarvestOutput(TechType.Cryptosuchus, CryptosuchusScaleID);
            CraftDataHandler.SetHarvestType(TechType.Cryptosuchus, HarvestType.DamageAlive);

            CraftDataHandler.SetHarvestOutput(TechType.OxygenPlant, TechType.PurpleBrainCoralPiece);
            CraftDataHandler.SetHarvestType(TechType.OxygenPlant, HarvestType.DamageAlive);

            CraftDataHandler.SetHarvestOutput(TechType.ShadowLeviathan, TechType.Kyanite);
            CraftDataHandler.SetHarvestType(TechType.ShadowLeviathan, HarvestType.DamageAlive);

            CraftDataHandler.SetHarvestOutput(TechType.IceWorm, TechType.Kyanite);
            CraftDataHandler.SetHarvestType(TechType.IceWorm, HarvestType.DamageAlive);
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

    class SquidSharkScale : DummySuitItems
    {
        public SquidSharkScale()
            : base(classID: "squidsharkscale", friendlyName: "Squid Shark Scale", description: "A scale from the head of a Squid Shark. Has uses in depth-resistant fabrication.")
        {
            OnFinishedPatching += SetStaticTechType;
        }

        protected override TechType BaseType { get; } = TechType.StalkerTooth;

        public override string AssetsFolder { get; } = @"BZNitrogenMod/Assets";

        private void SetStaticTechType() => SquidSharkScaleID = this.TechType;
    }

    class CryptosuchusScale : DummySuitItems
    {
        public CryptosuchusScale()
            : base(classID: "cryptosuchusscale", friendlyName: "Cryptosuchus Scale", description: "A scale from a Cryptosuchus. Has uses in depth and heat resistant fabrication.")
        {
            OnFinishedPatching += SetStaticTechType;
        }

        protected override TechType BaseType { get; } = TechType.StalkerTooth;

        public override string AssetsFolder { get; } = @"BZNitrogenMod/Assets";

        private void SetStaticTechType() => CryptosuchusScaleID = this.TechType;
    }

    class ThermophileSample : DummySuitItems
    {
        public ThermophileSample()
            : base(classID: "thermophilesample", friendlyName: "Thermophile Bacterial Sample", description: "A viable sample of a unique thermophile bacteria found on Rock Punchers belly. Undergoes chemosynthesis at high temperatures.")
        {
            OnFinishedPatching += SetStaticTechType;
        }

        protected override TechType BaseType { get; } = TechType.StalkerTooth;

        public override string AssetsFolder { get; } = @"BZNitrogenMod/Assets";

        private void SetStaticTechType() => ThermoBacteriaID = this.TechType;
    }
}