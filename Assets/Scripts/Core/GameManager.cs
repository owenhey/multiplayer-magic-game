using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Managing;
using FishNet.Managing.Logging;
using FishNet.Object;
using PlayerScripts;
using UnityEngine;

namespace Core {
    public class GameManager : NetworkBehaviour {
        private static GameManager _instance;

        public static GameManager Instance {
            get {
                if (_instance == null) {
                    _instance = GameObject.FindObjectOfType<GameManager>(true);
                }

                return _instance;
            }
            set {
                _instance = value;
            }
        }

        [SerializeField] private GamePlayerSpawner _playerSpawner;
        
        [Server]
        private void PlayerConnectedHandler(Player p) {
            p.PlayerReferences.PlayerStats.OnServerPlayerDeath += ()=> PlayerDeathHandler(p);
        }

        [Server]
        private void PlayerDeathHandler(Player p) {
            Debug.Log($"How many times does this run?");
            _playerSpawner.RespawnAfterTime(p);
        }

        [Server]
        public override void OnStartServer() {
            base.OnStartServer();
            Player.ServerOnPlayerConnected += PlayerConnectedHandler;
        }
        
        [Server]
        public override void OnStopServer() {
            base.OnStopServer();
            Player.ServerOnPlayerConnected -= PlayerConnectedHandler;
        }
        
        [Server(Logging = LoggingType.Off)]
        private void Awake() {
            Instance = this;
        }
    }
}
