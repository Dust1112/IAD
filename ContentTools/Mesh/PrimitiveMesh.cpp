#include "PrimitiveMesh.hpp"

#include "PrimitiveInitInfo.hpp"

#include "Geometry/Scene.hpp"
#include "Geometry/Geometry.hpp"

namespace iad::tools
{
    namespace
    {
        using primitive_mesh_creator = void(*)(Scene&, const PrimitiveInitInfo& info);

        void CreatePlane(Scene& scene, const PrimitiveInitInfo& info);
        void CreateCube(Scene& scene, const PrimitiveInitInfo& info);
        void CreateUvSphere(Scene& scene, const PrimitiveInitInfo& info);
        void CreateIcoSphere(Scene& scene, const PrimitiveInitInfo& info);
        void CreateCylinder(Scene& scene, const PrimitiveInitInfo& info);
        void CreateCapsule(Scene& scene, const PrimitiveInitInfo& info);

        primitive_mesh_creator creators[]
        {
            CreatePlane,
            CreateCube,
            CreateUvSphere,
            CreateIcoSphere,
            CreateCylinder,
            CreateCapsule,
        };

        static_assert(_countof(creators) == PrimitiveMeshType::count);
    }

    EDITOR_INTERFACE void CreatePrimitiveMesh(SceneData* data, PrimitiveInitInfo* info)
    {
        assert(data && info);
        assert(info->type < PrimitiveMeshType::count);

        SceneData scene{};
    }
}
