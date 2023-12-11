float GetRandomDistanceToCamera(InstanceShaderData instanceData, float random)
{
	float offsetDistanceToCamera = GetRandomLerp(distanceRandomOffset, random);

	return GetDistanceToCamera(instanceData.Position()) + offsetDistanceToCamera;
}

bool Culling(float distanceToCamera, float minDistance, float maxDistance)
{
	if(isDistanceCulling)
	{
		if (distanceToCamera > maxDistance)
		{
			return true;
		}
	}
	
	if (distanceToCamera < minDistance)
	{
		return true;
	}

	return false;
}

bool DistanceCulling(InstanceShaderData instanceData, float maxScaleValue, float random)
{
	float minDistanceCulling = minDistance;

	if(startLOD != 0)
	{
		minDistanceCulling *= maxScaleValue;
	}
	
	return Culling(GetRandomDistanceToCamera(instanceData, random), minDistanceCulling, maxDistance);
}