sampler2D _MainTex;
sampler2D _Bump; 
fixed4 _Color;
float _CullDistance;
float _FarCullDistance;
float _Brightness;
float _MipmapBias;
int _InRow;
int _InCol;
float4 _CameraPosition;

float _Smoothness;
float _Specular;
float _Occlusion;
float _NormalStrength;

struct Input
{
	float2 uv_MainTex;
	float4 d;
};

UNITY_INSTANCING_BUFFER_START(Props)

UNITY_INSTANCING_BUFFER_END(Props)

void vert(inout appdata_full v, out Input o)
{
	UNITY_INITIALIZE_OUTPUT(Input, o);

	float3 CENTER = v.normal;

    float3 worldspaceCenter = mul(unity_ObjectToWorld, CENTER);
    float3 modifiedCameraPos;
    if (_InCol == 1)
    {
        modifiedCameraPos =  _WorldSpaceCameraPos;
        modifiedCameraPos.y = worldspaceCenter.y;
    }
    else
    {
       modifiedCameraPos =  _WorldSpaceCameraPos.xyz;
    }

	float distanceToCamera = length(worldspaceCenter - _CameraPosition);

#if defined(UNITY_PASS_SHADOWCASTER)
	float3 camVect;

	if (unity_MatrixVP[3][3] == 1)
		camVect = _WorldSpaceLightPos0.w < 0.5 ? _WorldSpaceLightPos0.xyz : worldspaceCenter - _WorldSpaceLightPos0.xyz;
	else
		camVect = worldspaceCenter - modifiedCameraPos;

#define camVectEvenInShadows (worldspaceCenter - cameraPos)			
#else
	float3 camVect = worldspaceCenter - modifiedCameraPos;
#define camVectEvenInShadows camVect

#endif
	
	if (distanceToCamera < _CullDistance || distanceToCamera > _FarCullDistance)
	{
		v.vertex.xyz *= 0;
	}
	else
	{
		// Create LookAt matrix
		float3 zaxis = normalize(camVect);
		float3 xaxis = normalize(cross(float3(0, 1, 0), zaxis));
		float3 yaxis = cross(zaxis, xaxis);

		float4x4 lookatMatrix = {
			xaxis.x,            yaxis.x,            zaxis.x,       0,
			xaxis.y,            yaxis.y,            zaxis.y,       0,
			xaxis.z,            yaxis.z,            zaxis.z,       0,
			0, 0, 0,  1
		};

		float3 initialVertex = v.vertex - CENTER.xyz;

		v.vertex = mul(lookatMatrix, float4(initialVertex.x, initialVertex.y, initialVertex.z, 1));
		v.vertex.xyz += CENTER.xyz;
		
		v.normal = -zaxis;
		v.tangent.xyz = xaxis;
		v.tangent.w = -1;

		v.texcoord.x /= _InRow;
		v.texcoord.y /= _InCol;

		float angle;
		float step;
		float2 atanDir = normalize(float2(-zaxis.z, -zaxis.x));
		angle = (atan2(atanDir.y, atanDir.x) / 6.28319) + 0.5; // angle around Y in range 0....1
		angle += v.texcoord1.x;
		angle -= (int)angle;
		step = 1.0 / _InRow;
		v.texcoord.x += step * ((int)((angle + step * 0.5) * _InRow));
		step = 1.0 / _InCol;
		angle = saturate(dot(-zaxis, float3(0, 1, 0)));
		angle = clamp(angle, 0, step*(_InCol - 1));
		v.texcoord.y += step * ((int)((angle + step * 0.5) * _InCol));
		o.d.x = v.texcoord1.y;
	}
}

void surfSpecular(Input IN, inout SurfaceOutputStandardSpecular o)
{
	fixed4 c = tex2Dbias(_MainTex, half4(IN.uv_MainTex, 0, _MipmapBias)) * _Color;

	o.Albedo = c.rgb * IN.d.x *_Color;
	o.Albedo = clamp(o.Albedo * _Brightness, 0, 1);
	o.Normal = tex2D(_Bump, IN.uv_MainTex).rgb * 2.0 - 1.0;
	
	o.Normal.xy *= _NormalStrength;
	
	o.Occlusion = _Occlusion; 
	o.Smoothness = _Smoothness;
	o.Specular = _Specular;
	o.Alpha = c.a;
}