// vk, fb, ok, tw, wa, vb, tg
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class Sharing : MonoBehaviour {


	private string vkTemplate = "http://vk.com/share.php?title={0}&description={1}&image={2}&url={3}";
	private string facebookTemplate = "https://www.facebook.com/sharer/sharer.php?u={3}";
	private string odnoklassnikiTemplate = "http://www.odnoklassniki.ru/dk?st.cmd=addShare&st.s=1&st._surl={3}&st.comments={1}";
	private string twitterTemplate = "https://twitter.com/intent/tweet?text={0}&url={3}";

	static string socialType;
	static string msg;

	public static string gameLink = "https://play.google.com/store/apps/details?id=???";

	#if UNITY_IOS
		[DllImport ("__Internal")]
		private static extern void shareVia (string app, string message, string url, string param);
	#endif

	// Поделиться 
	public static void ShareVia (string app, string message, string param = "") {
		socialType = app;
		msg = message;
		message = message + "\n" + gameLink;
		#if UNITY_ANDROID
			using (var plugin = new AndroidJavaClass("com.mycompany.sharing.Plugin")) {
				plugin.CallStatic("shareVia", app, message);
			}
		#elif UNITY_IOS
			shareVia (app, message, "http://my.url.com", param);
		#endif
	}

	// Не удалось расшарить
	void OnShareError (string result) {
		switch (result) {
		case "NotInstall":
			// приложение не установлено
			Debug.Log ("приложение " + socialType + " не установлено");
			switch(socialType) {
			case "vk":
				Application.OpenURL(string.Format("http://vk.com/share.php?description={0}&url={1}", msg, gameLink));
				break;

			case "tw":
				Application.OpenURL(string.Format("https://twitter.com/intent/tweet?text={0}&url={1}", msg, gameLink));
				break;

			case "fb":
				Application.OpenURL(string.Format("https://www.facebook.com/sharer/sharer.php?description={0}&u={1}","",msg,gameLink));
				break;
			}

			break;
		case "NotAvailable":
			// шаринг не доступен
			Debug.Log ("шаринг не доступен");
			break;
		case "AccessDenied":
			// нет доступа
			Debug.Log ("нет доступа");
			break;
		default:
			// не удалось расшарить текст
			Debug.Log ("не удалось расшарить");
			break;
		}
	}
}
