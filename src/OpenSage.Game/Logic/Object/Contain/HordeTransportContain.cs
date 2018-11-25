﻿using OpenSage.Data.Ini;
using OpenSage.Data.Ini.Parser;

namespace OpenSage.Logic.Object
{
    [AddedIn(SageGame.Bfme)]
    public class HordeTransportContainModuleData : BehaviorModuleData
    {
        internal static HordeTransportContainModuleData Parse(IniParser parser) => parser.ParseBlock(FieldParseTable);

        internal static readonly IniParseTable<HordeTransportContainModuleData> FieldParseTable = new IniParseTable<HordeTransportContainModuleData>
        {
           { "ObjectStatusOfContained", (parser, x) => x.ObjectStatusOfContained = parser.ParseEnumBitArray<ObjectStatus>() },
           { "Slots", (parser, x) => x.Slots = parser.ParseInteger() },
           { "EnterSound", (parser, x) => x.EnterSound = parser.ParseAssetReference() },
           { "ExitSound", (parser, x) => x.ExitSound = parser.ParseAssetReference() },
           { "DamagePercentToUnits", (parser, x) => x.DamagePercentToUnits = parser.ParsePercentage() },
           { "PassengerFilter", (parser, x) => x.PassengerFilter = ObjectFilter.Parse(parser) },
           { "AllowEnemiesInside", (parser, x) => x.AllowEnemiesInside = parser.ParseBoolean() },
           { "AllowNeutralInside", (parser, x) => x.AllowNeutralInside = parser.ParseBoolean() },
           { "ExitDelay", (parser, x) => x.ExitDelay = parser.ParseInteger() },
           { "NumberOfExitPaths", (parser, x) => x.NumberOfExitPaths = parser.ParseInteger() },
           { "ForceOrientationContainer", (parser, x) => x.ForceOrientationContainer = parser.ParseBoolean() },
           { "PassengerBonePrefix", (parser, x) => x.PassengerBonePrefix = PassengerBonePrefix.Parse(parser) },
           { "EjectPassengersOnDeath", (parser, x) => x.EjectPassengersOnDeath = parser.ParseBoolean() },
        };

        public BitArray<ObjectStatus> ObjectStatusOfContained { get; private set; }
        public int Slots { get; private set; }
        public string EnterSound { get; private set; }
        public string ExitSound { get; private set; }
        public float DamagePercentToUnits { get; private set; }
        public ObjectFilter PassengerFilter { get; private set; }
        public bool AllowEnemiesInside { get; private set; }
        public bool AllowNeutralInside { get; private set; }
        public int ExitDelay { get; private set; }
        public int NumberOfExitPaths { get; private set; }
        public bool ForceOrientationContainer { get; private set; }
        public PassengerBonePrefix PassengerBonePrefix { get; private set; }
        public bool EjectPassengersOnDeath { get; private set; }
    }
}