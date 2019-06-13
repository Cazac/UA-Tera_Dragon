using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class UpgradeNode : ScriptableObject {
    public string upgradeName;
    private UpgradeType upgradeType;
    public bool isActive;
    public UpgradeNode[] necessaryNodes;

    public void ActivateUpgrade() {
        foreach(UpgradeNode node in necessaryNodes) {
            if (!node.isActive)
                return;
        }
        isActive = true;
    }
    
    public UpgradeType GetUpgradeType() {
        return upgradeType;
    }
    
}

public enum UpgradeType {
    STATS_MODIFIER,
    EFFECT,
}
