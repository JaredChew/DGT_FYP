using System.Collections.Generic;

[System.Serializable] public class SaveData {

    private int currentArea = 0;
    private int currentScenario = 0;
    private int lastCheckPointAt = 0;

    private bool portalActive = false;


    public SaveData(int lastCheckPointAt, int currentArea, int currentScenario, bool portalActive) {

        this.currentArea = currentArea;
        this.currentScenario = currentScenario;
        this.lastCheckPointAt = lastCheckPointAt;

        this.portalActive = portalActive;

    }

    public int getLastCheckPointAt() {
        return lastCheckPointAt;
    }

    public int getCurrentScenario() {
        return currentScenario;
    }

    public int getCurrentArea() {
        return currentArea;
    }

    public bool getPortalActive() {
        return portalActive;
    }

}
