﻿using System.Collections.Generic;
using System.IO;
using OpenSage.FileFormats;

namespace OpenSage.Data.Map
{
    public sealed class ScriptList : Asset
    {
        public const string AssetName = "ScriptList";

        public ScriptGroup[] ScriptGroups { get; private set; }
        public Script[] Scripts { get; private set; }

        internal static ScriptList Parse(BinaryReader reader, MapParseContext context)
        {
            return ParseAsset(reader, context, version =>
            {
                if (version != 1)
                {
                    throw new InvalidDataException();
                }

                var scriptGroups = new List<ScriptGroup>();
                var scripts = new List<Script>();

                ParseAssets(reader, context, assetName =>
                {
                    switch (assetName)
                    {
                        case ScriptGroup.AssetName:
                            scriptGroups.Add(ScriptGroup.Parse(reader, context));
                            break;

                        case Script.AssetName:
                            scripts.Add(Script.Parse(reader, context));
                            break;

                        default:
                            throw new InvalidDataException($"Unexpected asset: {assetName}");
                    }
                });
                
                return new ScriptList
                {
                    ScriptGroups = scriptGroups.ToArray(),
                    Scripts = scripts.ToArray()
                };
            });
        }

        internal void WriteTo(BinaryWriter writer, AssetNameCollection assetNames)
        {
            WriteAssetTo(writer, () =>
            {
                foreach (var script in Scripts)
                {
                    writer.Write(assetNames.GetOrCreateAssetIndex(Script.AssetName));
                    script.WriteTo(writer, assetNames);
                }

                foreach (var scriptGroup in ScriptGroups)
                {
                    writer.Write(assetNames.GetOrCreateAssetIndex(ScriptGroup.AssetName));
                    scriptGroup.WriteTo(writer, assetNames);
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

            var numScriptGroups = reader.ReadUInt16();

            if (numScriptGroups != ScriptGroups.Length)
            {
                throw new InvalidDataException();
            }

            for (var i = 0; i < numScriptGroups; i++)
            {
                ScriptGroups[i].Load(reader);
            }
        }
    }
}
