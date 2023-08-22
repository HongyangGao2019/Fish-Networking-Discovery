using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace FishNet.Discovery
{
	public sealed class NetworkDiscoveryHUD : MonoBehaviour
	{
		[SerializeField]
		private NetworkDiscovery networkDiscovery;

		private readonly List<IPEndPoint> _endPoints = new List<IPEndPoint>();

		private Vector2 _serversListScrollVector;

		private void Start()
		{
			if (networkDiscovery == null) networkDiscovery = FindObjectOfType<NetworkDiscovery>();

			networkDiscovery.ServerFoundCallback += endPoint =>
			{
				if (!_endPoints.Contains(endPoint)) _endPoints.Add(endPoint);
			};
		}

		private void OnGUI()
		{
			GUILayoutOption buttonHeight = GUILayout.Height(30.0f);

			using (new GUILayout.AreaScope(new Rect(Screen.width - 240.0f - 10.0f, 10.0f, 240.0f, Screen.height - 20.0f)))
			{
				GUILayout.Box("Server");

				using (new GUILayout.HorizontalScope())
				{
					if (GUILayout.Button("Start", buttonHeight)) InstanceFinder.ServerManager.StartConnection();

					if (GUILayout.Button("Stop", buttonHeight)) InstanceFinder.ServerManager.StopConnection(true);
				}

				GUILayout.Box("Advertising");

				using (new GUILayout.HorizontalScope())
				{
					if (networkDiscovery.IsAdvertising)
					{
						if (GUILayout.Button("Stop", buttonHeight)) networkDiscovery.StopSearchingOrAdvertising();
					}
					else
					{
						if (GUILayout.Button("Start", buttonHeight)) networkDiscovery.AdvertiseServer();
					}
				}

				GUILayout.Box("Searching");

				using (new GUILayout.HorizontalScope())
				{
					if (networkDiscovery.IsSearching)
					{
						if (GUILayout.Button("Stop", buttonHeight)) networkDiscovery.StopSearchingOrAdvertising();
					}
					else
					{
						if (GUILayout.Button("Start", buttonHeight)) networkDiscovery.SearchForServers();
					}
				}

				if (_endPoints.Count < 1) return;

				GUILayout.Box("Servers");
				
				_serversListScrollVector = GUILayout.BeginScrollView(_serversListScrollVector);
                
				foreach (IPEndPoint endPoint in _endPoints)
				{
					string ipAddress = endPoint.Address.ToString();

					if (!GUILayout.Button(ipAddress)) continue;

					networkDiscovery.StopSearchingOrAdvertising();

					InstanceFinder.ClientManager.StartConnection(ipAddress);
				}

				GUILayout.EndScrollView();
			}
		}
	}
}
