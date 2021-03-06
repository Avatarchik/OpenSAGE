﻿using System.Collections.Generic;
using System.IO;
using OpenSage.Data.Ini;
using OpenSage.Data.Utilities.Extensions;
using OpenSage.FileFormats;

namespace OpenSage.Data.Map
{
    public sealed class ScriptGroup : Asset
    {
        public const string AssetName = "ScriptGroup";

        public string Name { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsSubroutine { get; private set; }
        public Script[] Scripts { get; private set; }

        public ScriptGroup[] Groups { get; private set; }

        internal static ScriptGroup Parse(BinaryReader reader, MapParseContext context)
        {
            return ParseAsset(reader, context, version =>
            {
                var name = reader.ReadUInt16PrefixedAsciiString();
                var isActive = reader.ReadBooleanChecked();
                var isSubroutine = reader.ReadBooleanChecked();

                var scripts = new List<Script>();
                var groups = new List<ScriptGroup>();

                ParseAssets(reader, context, assetName =>
                {
                    switch (assetName)
                    {
                        case ScriptGroup.AssetName:
                            if (version < 3)
                            {
                                goto default;
                            }
                            groups.Add(ScriptGroup.Parse(reader, context));
                            break;

                        case Script.AssetName:
                            scripts.Add(Script.Parse(reader, context));
                            break;

                        default:
                            throw new InvalidDataException($"Unexpected asset: {assetName}");
                    }
                });

                return new ScriptGroup
                {
                    Name = name,
                    IsActive = isActive,
                    IsSubroutine = isSubroutine,
                    Scripts = scripts.ToArray(),
                    Groups = groups.ToArray()
                };
            });
        }

        internal void WriteTo(BinaryWriter writer, AssetNameCollection assetNames)
        {
            WriteAssetTo(writer, () =>
            {
                writer.WriteUInt16PrefixedAsciiString(Name);
                writer.Write(IsActive);
                writer.Write(IsSubroutine);

                foreach (var scriptGroup in Groups)
                {
                    writer.Write(assetNames.GetOrCreateAssetIndex(ScriptGroup.AssetName));
                    scriptGroup.WriteTo(writer, assetNames);
                }

                foreach (var script in Scripts)
                {
                    writer.Write(assetNames.GetOrCreateAssetIndex(Script.AssetName));
                    script.WriteTo(writer, assetNames);
                }
            });
        }

        internal void Load(BinaryReader reader)
        {
            var version = reader.ReadVersion();

            var numScripts = reader.ReadUInt16();

            if (numScripts != Scripts.Length)
            {
                throw new InvalidDataException();
            }

            for (var i = 0; i < numScripts; i++)
            {
                Scripts[i].Load(reader);
            }
        }
    }
}
