using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using Net;
using PlayerScripts;
using UnityEngine;

namespace Core {
    public class GamePlayerSpawner : MonoBehaviour {
        [Server]
        public void RespawnAfterTime(Player player) {
            StartCoroutine(RespawnAfterTimeC(player));
        }

        [Server]
        private IEnumerator RespawnAfterTimeC(Player player) {
            ServerChat.Instance.SendMessageToClient($"Respawning in 3", player);
            yield return new WaitForSeconds(1.0f);
            ServerChat.Instance.SendMessageToClient($"Respawning in 2", player);
            yield return new WaitForSeconds(1.0f);
            ServerChat.Instance.SendMessageToClient($"Respawning in 1", player);
            yield return new WaitForSeconds(1.0f);
            player.PlayerReferences.PlayerStats.ServerSpawnPlayer();
        }
    }
}