using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Managing;
using FishNet.Object;
using PlayerScripts;
using UnityEngine;

namespace Core {
    public class GameManager : NetworkBehaviour {
        private static GameManager _instance;

        public static GameManager Instance {
            get {
                if (_instance == null) {
                    _instance = GameObject.FindObjectOfType<GameManager>();
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
            p.PlayerReferences.PlayerStats.OnPlayerDeath += ()=> PlayerDeathHandler(p);
        }

        [Server]
        private void PlayerDeathHandler(Player p) {
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
        
        [Server]
        private void Awake() {
            Instance = this;
        }
    }
}
