using UnityEngine;
using Battlehub.RTEditor;
using Battlehub.RTCommon;
using System.Threading.Tasks;
using System.IO;
using Battlehub.RTSL.Interface;
using GLTFast;
using System;

using UnityObject = UnityEngine.Object;
using System.Threading;
using Battlehub.RTEditor.Models;

namespace Battlehub.RTImporter
{
    public class GlbImporterAsync : GltfImporterAsync
    {
        public override string FileExt
        {
            get { return ".glb"; }
        }
    }

    public class GltfImporterAsync : ProjectFileImporterAsync
    {
        public override int Priority
        {
            get { return int.MinValue; }
        }

        public override string FileExt
        {
            get { return ".gltf"; }
        }

        public override string IconPath
        {
            get { return "Importers/GLTF"; }
        }

        public override Type TargetType
        {
            get { return typeof(GameObject); }
        }

        public override async Task ImportAsync(string filePath, string targetPath, IProjectAsync project, CancellationToken cancelToken)
        {
            var gltf = new GltfImport();

            var settings = new ImportSettings
            {
                generateMipMaps = true,
                anisotropicFilterLevel = 3,
                nodeNameMethod = ImportSettings.NameImportMethod.OriginalUnique
            };

            GameObject go = new GameObject(Path.GetFileNameWithoutExtension(filePath));
            try
            {
                bool success = await gltf.Load(filePath, settings);
                if (!success)
                {
                    throw new FileImporterException("Import failed");
                }

                go.SetActive(false);
                gltf.InstantiateMainScene(go.transform);

                IRuntimeEditor editor = IOC.Resolve<IRuntimeEditor>();
                ProjectItem folder = project.Utils.GetFolder(Path.GetDirectoryName(targetPath));

                Transform[] children = go.GetComponentsInChildren<Transform>(true);
                for (int i = 0; i < children.Length; ++i)
                {
                    children[i].gameObject.AddComponent<ExposeToEditor>();

                    if(i % 100 == 0)
                    {
                        await Task.Yield();
                    }
                }

                await editor.CreatePrefabAsync(folder, go.GetComponent<ExposeToEditor>(), true);
            }
            catch(Exception e)
            {
                throw new FileImporterException(e.Message, e);
            }
            finally
            {
                UnityObject.Destroy(go);
            }
        }

    }
}
