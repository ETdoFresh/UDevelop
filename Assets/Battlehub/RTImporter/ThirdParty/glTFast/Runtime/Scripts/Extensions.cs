// Copyright 2020-2021 Andreas Atteneder
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

namespace GLTFast {
    
    /// <summary>
    /// Collection of glTF extension names
    /// </summary>
    public static class Extensions {
        public const string DracoMeshCompression = "KHR_draco_mesh_compression";
        public const string MaterialsPbrSpecularGlossiness = "KHR_materials_pbrSpecularGlossiness";
        public const string MaterialsTransmission = "KHR_materials_transmission";
        public const string MaterialsUnlit = "KHR_materials_unlit";
        public const string MeshGPUInstancing = "EXT_mesh_gpu_instancing";
        public const string MeshQuantization = "KHR_mesh_quantization";
        public const string TextureBasisUniversal = "KHR_texture_basisu";
        public const string TextureTransform = "KHR_texture_transform";
    }
}
