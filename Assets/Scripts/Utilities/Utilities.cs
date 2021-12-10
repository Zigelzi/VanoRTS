using UnityEngine;

public static class VanoUtilities
{
    public static Color GenerateRandomColor()
    {
        float red = Random.Range(0, 1f);
        float green = Random.Range(0, 1f);
        float blue = Random.Range(0, 1f);

        Color color = new Color(red, green, blue);
        return color;
    }
}
