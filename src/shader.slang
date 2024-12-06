struct Vertex
{
    float3 position;
    float3 normal;
    float3 color;
    float2 uv;
}

struct Material
{
    int albedoTextureIndex;
    int normalTextureIndex;
}

struct Scene
{
    StructuredBuffer<Material> materials;
    Texture2D textures[128];
    float4x4 modelMatrix;
    float4x4 modelInverseMatrix;
}

struct Camera
{
    float4x4 viewProjection;
    float3 cameraPosition;
}

ParameterBlock<Scene> scene;
ConstantBuffer<Camera> camera;
SamplerState samplerState;

struct VOut
{
    float4 position : SV_Position;
    float3 worldPosition;
    float3 normal : NORMAL;
    float3 worldNormal : NORMAL;
    float2 uv : TEXCOORD;
}

[shader("vertex")]
VOut vertexMain(Vertex input)
{
    VOut output;

    float3 position = input.position;
    float3 normal = input.normal;
    float2 uv = input.uv;

    float3 worldPosition = mul(scene.modelMatrix, float4(position, 1.0)).xyz;
    float3 worldNormal = mul(scene.modelInverseMatrix, float4(normal, 0.0)).xyz;

    output.position = mul(camera.viewProjection, float4(input.position, 1.0));
    output.normal = normal;
    output.uv = uv;
    return output;
}

[shader("fragment")]
float4 fragmentMain(VOut input) : SV_Target
{
    let texture = scene.textures[scene.materials[0].albedoTextureIndex];
    let color = texture.Sample(samplerState, input.uv);
    return color;
}
