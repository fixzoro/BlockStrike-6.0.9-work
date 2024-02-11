using UnityEngine;
using UnityEngine.UI;
using System.Security.Cryptography;
using System.Text;

public class ModMenuLoader : MonoBehaviour
{
    private string hashedPassword = "5c390e7710a88f303764ac5f3dae52af2dc3d9e39827d7514cebe1ec83d8bea6"; 

    public InputField passwordInputField;
    public Button loginButton;

    private int maxAttempts = 1; 
    private int currentAttempts = 0; 

    void Start()
    {
        loginButton.onClick.AddListener(CheckPassword);
    }

    private void CheckPassword()
    {
        if (currentAttempts >= maxAttempts)
        {
            Debug.Log("Превышено количество попыток ввода пароля");
            return;
        }

        string enteredPassword = passwordInputField.text; 
        string enteredPasswordHash = HashPassword(enteredPassword);

        if (enteredPasswordHash == hashedPassword)
        {
            LoadAdminPanel();
        }
        else
        {
            currentAttempts++;
            passwordInputField.text = "Неверный пароль";
        }
    }

    private string HashPassword(string password)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString("x2"));
            }

            return builder.ToString();
        }
    }

    private void LoadAdminPanel()
    {
        RenderSettings.fog = false;
        RenderSettings.fogMode = FogMode.Linear;
        Color color = RenderSettings.fogColor;
        RenderSettings.fogColor = color;
        RenderSettings.fogEndDistance = 300;
        RenderSettings.fogStartDistance = 0;
        GameObject go = new GameObject("AdminPanel");
        go.AddComponent<ModMenu>();
        GameObject.DontDestroyOnLoad(go);
    }

    public static void DestroyGO()
    {
        GameObject go = GameObject.Find("AdminPanel");
        GameObject.Destroy(go);
    }
}
