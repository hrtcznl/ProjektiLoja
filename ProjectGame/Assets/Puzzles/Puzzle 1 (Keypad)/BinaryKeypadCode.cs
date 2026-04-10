using UnityEngine;
using TMPro;

public class BinaryKeypadCode : MonoBehaviour
{
    public TextMeshPro textElement;

    void Start()
    {
        textElement.text = GenerateValidBinaryGroups();
    }

    string GenerateValidBinaryGroups()
    {
        string result = "";

        for (int i = 0; i < 4; i++) // 4 digits
        {
            string group = GenerateValidGroup();
            result += group;

            if (i < 3)
                result += " ";
        }

        return result;
    }

    string GenerateValidGroup()
    {
        while (true)
        {
            string binary = "";

            for (int i = 0; i < 4; i++)
            {
                binary += Random.Range(0, 2).ToString();
            }

            int value = System.Convert.ToInt32(binary, 2);

            if (value <= 9) // only allow 0–9
                return binary;
        }
    }
}