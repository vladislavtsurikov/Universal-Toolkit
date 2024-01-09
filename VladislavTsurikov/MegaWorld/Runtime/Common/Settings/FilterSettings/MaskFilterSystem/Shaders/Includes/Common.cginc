float InverseLerp(float a, float b, float t)
{
    return (t - a) / (b - a);
}

float Remap(float height, float remapMin, float remapMax)
{
    float value;

    if (height < remapMin)
    {
        value = 0;
    }
    else if (height > remapMax)
    {
        value = 1;
    }
    else
    {
        value = InverseLerp(remapMin, remapMax, height);
    }

    return float4(value, value, value, 1);
}