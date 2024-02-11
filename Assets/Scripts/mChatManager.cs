using System;
using BestHTTP;
using BestHTTP.ServerSentEvents;
using FreeJSON;
using UnityEngine;
using UnityEngine.UI; // Добавьте эту строку для использования InputField

public class mChatManager : MonoBehaviour
{
    public InputField input; // Изменено с UIInput на InputField

    public UITextList textList;

    private float time;

    private float maxTime;

    private string link;

    private HTTPRequest request;

    private EventSource sse;

    private JsonObject json = new JsonObject();

    private bool isOpen;

    public void Open()
    {
        if (!AccountManager.isConnect)
        {
            UIToast.Show(Localization.Get("Connection account"));
            return;
        }
        textList.Clear();
        mPanelManager.Show("GlobalChat", true);
        if (sse == null)
        {
            sse = new EventSource(new Uri("block-strike-a8132-default-rtdb.firebaseio.com"));
            sse.Open();
            sse.OnOpen += delegate
            {
                isOpen = true;
            };
            sse.OnError += delegate (EventSource eventSource, string error)
            {
                UIToast.Show("Global Chat Error: " + error);
            };
            sse.OnClosed += delegate
            {
                isOpen = false;
            };
            sse.OnMessage += OnMessage;
            sse.OnRetry += OnRetry;
        }
        else
        {
            sse.Open();
        }
    }

    private void OnMessage(EventSource source, Message message)
    {
        if (string.IsNullOrEmpty(message.Data) || message.Data == "null" || !JsonObject.isJson(message.Data))
        {
            return;
        }
        json = JsonObject.Parse(message.Data);
        string text = json.Get<string>("data");
        if (string.IsNullOrEmpty(text) || text == "null")
        {
            return;
        }
        if (JsonObject.isJson(text))
        {
            JsonObject jsonObject = json.Get<JsonObject>("data");
            for (int i = 0; i < jsonObject.Length; i++)
            {
                textList.Add(jsonObject.Get<string>(jsonObject.GetKey(i)));
            }
        }
        else
        {
            textList.Add(text);
        }
    }

    public void Close()
    {
        bool flag = this.sse != null;
        if (flag)
        {
            this.sse.Close();
            HTTPManager.AbortAll();
            sse = null;
        }
    }

    private bool OnRetry(EventSource source)
    {
        return true;
    }

    public void OnSubmit()
    {
        if (this.isOpen)
        {
            if (this.time + this.maxTime > Time.time)
            {
                this.textList.Add("Message sending limit " + StringCache.Get(Mathf.CeilToInt(this.time + this.maxTime - Time.time)) + " sec");
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Return)) // Добавлена проверка на нажатие клавиши Enter
                {
                    this.Add(this.input.text); // Используем input.text для получения текста из InputField
                }
            }
        }
    }

    private void Add(string text)
    {
        if (!string.IsNullOrEmpty(text) && !isOnlySpace(text))
        {
            if (text.Contains("\n"))
            {
                text = text.Replace("\n", string.Empty);
            }
            text = BadWordsManager.Check(text);
            if (time + maxTime + 2f > Time.time)
            {
                maxTime += 1f;
            }
            else
            {
                maxTime = 0f;
            }
            time = Time.time;
            text = AccountManager.instance.Data.AccountName + ": " + NGUIText.StripSymbols(text);
            input.text = string.Empty; // Используем input.text для очистки содержимого поля ввода
            Firebase firebase = new Firebase();
            firebase.Auth = AccountManager.AccountToken;
            JsonObject jsonObject = new JsonObject();
            long epochTicks = new DateTime(1970, 1, 1).Ticks;
            long unixTime = ((DateTime.UtcNow.Ticks - epochTicks) / TimeSpan.TicksPerSecond);
            jsonObject.Add(unixTime.ToString(), text);
            firebase.Child("GlobalChat").UpdateValue(jsonObject.ToString(), delegate
            {
            }, null);
            TimerManager.In(5f, delegate
            {
                StopAllCoroutines();
            });
        }
    }

    private bool isOnlySpace(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            bool flag = text[i] != ' ';
            if (flag)
            {
                return false;
            }
        }
        return true;
    }
}
