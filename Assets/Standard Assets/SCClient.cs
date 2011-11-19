using UnityEngine;
using System.Collections;

using System;
using System.Net;
using System.Net.Sockets;

public class SCClient : MonoBehaviour {
	#region public fields
	public string scHostname = "localhost";
	public int scPort = 57110;
	public int listenPort = 6666;
	public GameObject[] listeners;
	#endregion

	#region private fields
	bool listen = true;
	UdpClient client;
	#endregion

	void Start () {
		client = new UdpClient(listenPort);
		client.Connect(scHostname,scPort);
		Send("/notify", 1);
		StartCoroutine( Listen() );
	}

	void OnApplicationQuit()
	{
		Send("/notify", 0);
		listen = false;
		StopAllCoroutines();
		client.Close();
	}

	class UdpState {
		UdpClient client;
		byte[] data = null;
		public byte[] Data { get { return data; } }

		public UdpState(UdpClient c)
		{
			client = c;
		}

		public void Begin() {
			data = null;
			client.BeginReceive(new AsyncCallback(EndCallback), this);
		}

		void End(IAsyncResult ar) {
			IPEndPoint tmp = client.Client.RemoteEndPoint as IPEndPoint;
			data = client.EndReceive(ar, ref tmp);
		}

		static void EndCallback(IAsyncResult ar)
		{
			(ar.AsyncState as UdpState).End(ar);
		}

	}

	IEnumerator Listen()
	{
		UdpState s = new UdpState(client);

		do {
			s.Begin();
			while(listen && s.Data == null) {
				yield return 1; // check again at next frame
			}
			if (!listen)
				break;
			object[] msg = Osc.ToArray(s.Data);
			foreach(GameObject listener in listeners)
				listener.BroadcastMessage("OnOscMessage", msg);

		} while(listen);
	}

	IPAddress ResolveIPString(string host)
	{
		IPAddress[] addrs = Dns.GetHostAddresses(host);
		if (addrs.Length <= 0) {
			Debug.LogError("Can't resolve ip address of " + host);
			return null;
		}
		return addrs[0];
	}

	void Send(params object[] payload)
	{
		byte[] data = Osc.FromArray(payload);
		client.Send(data, data.Length);
	}
}
