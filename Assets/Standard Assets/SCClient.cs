using UnityEngine;
using System.Collections;

using System;
using System.Net;
using System.Net.Sockets;

using Bespoke.Common.Osc;

public class SCClient : MonoBehaviour {

	public string scHostname = "localhost";
	public int scPort = 57110;

	public int listenPort = 6666;
	bool listen = true;

	IPEndPoint LocalEP { get { return client.Client.LocalEndPoint as IPEndPoint; } }
	IPEndPoint RemoteEP { get { return client.Client.RemoteEndPoint as IPEndPoint; } }
	UdpClient client;

	void Start () {
		OscPacket.LittleEndianByteOrder = false;
		client = new UdpClient(listenPort);
		client.Connect(scHostname,scPort);

		// start listening
		StartCoroutine( Listen() );

		// subscribe
		Send("/notify", 1);
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
		OscPacket data = null;
		public OscPacket Data { get { return data; } }

		public UdpState(UdpClient c) {
			client = c;
		}

		public void Begin() {
			data = null;
			client.BeginReceive(new AsyncCallback(EndCallback), this);
		}

		void End(IAsyncResult ar) {
			IPEndPoint tmp = client.Client.RemoteEndPoint as IPEndPoint;
			byte[] buffer = client.EndReceive(ar, ref tmp);
			data = OscPacket.FromByteArray( tmp, buffer );
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
			// react to Data
			if (s.Data.IsBundle) {
				Debug.LogWarning("FIXME bundle reception not implemented");
			} else {
				OscMessage m = s.Data as OscMessage;
				Debug.Log(m.Address + " " + m.ToString());
			}
		} while(listen);
		Debug.Log("Exiting UDP listen loop");
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

	static bool AppendIfTypeMatches<T>(OscMessage m, object o) {
		if (o.GetType() == typeof(T)) {
			m.Append<T>((T)o);
			return true;
		}
		return false;
	}

	public class OscIncompatibleTypeError : System.Exception {
		public OscIncompatibleTypeError(object o)
			: base("Unsupported OSC payload type: " + o.GetType().Name)
		{}
	}

	void Send(string path, params object[] payload)
	{
		OscMessage m = new OscMessage(LocalEP, path);
		foreach(object o in payload) {
			if (!AppendIfTypeMatches<int>(m,o) &&
			    !AppendIfTypeMatches<long>(m,o) &&
			    !AppendIfTypeMatches<float>(m,o) &&
			    !AppendIfTypeMatches<double>(m,o) &&
			    !AppendIfTypeMatches<string>(m,o) &&
			    !AppendIfTypeMatches<byte[]>(m,o))
			{
				throw new OscIncompatibleTypeError(o);
			}
		}
		byte[] data = m.ToByteArray();
		client.Send(data, data.Length);
	}
}
