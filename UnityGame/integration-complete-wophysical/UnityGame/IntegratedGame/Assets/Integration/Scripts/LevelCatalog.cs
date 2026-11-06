// Inside the Network - integration layer
// One list that describes every OSI layer game: which scene it lives in, plus the text
// shown on its info panel in Padma's level menu.
using System;

namespace InsideTheNetwork.Integration
{
    public sealed class LayerInfo
    {
        public readonly int Number;          // OSI layer number (7 = Application ... 1 = Physical)
        public readonly string Name;         // "Application"
        public readonly string Threat;       // panel title, e.g. "Exploit"
        public readonly string Scene;        // Unity scene name to load
        public readonly string Instructions; // panel instructions, same style as Padma's panels
        public readonly string MenuButton;   // name of this layer's button in Padma's LevelMenu (LevelList)

        public LayerInfo(int number, string name, string threat, string scene, string instructions, string menuButton)
        {
            Number = number;
            Name = name;
            Threat = threat;
            Scene = scene;
            Instructions = instructions;
            MenuButton = menuButton;
        }

        // "(An Application Layer Threat)", matching Padma's subheadings
        public string Subheading
        {
            get
            {
                string article = "AEIOU".IndexOf(char.ToUpperInvariant(Name[0])) >= 0 ? "An" : "A";
                return "(" + article + " " + Name + " Layer Threat)";
            }
        }
    }

    public static class LevelCatalog
    {
        // Padma's menu scenes are the front of the game.
        public const string MainMenuScene = "TitleScene";
        public const string LevelMenuScene = "LevelMenu";

        // Data travels DOWN the stack from the IoT device, so play order is 7 -> 1.
        // Scene names are the names already used inside each teammate's project.
        public static readonly LayerInfo[] Layers =
        {
            new LayerInfo(7, "Application", "Exploit", "Application_layer",
                "Use WASD to move and the mouse to look, E to pick up and deliver\nRoute each client request to the right protocol server",
                "AppButton"),
            new LayerInfo(6, "Presentation", "Phishing", "PhishingScene",
                "Use WASD/Arrow keys to move\nAvoid the walls and reach the destination before time runs out",
                "PreButton"),
            new LayerInfo(5, "Session", "Hijacking", "SessionLayer",
                "Use the mouse\nOpen sessions in the right order, pass the token and checkpoint transfers",
                "SesButton"),
            new LayerInfo(4, "Transport", "Denial of Service", "TransportLayer",
                "Use WASD to move, mouse to look, Space to jump\nCollect 3 TCP segments and 2 UDP datagrams while avoiding DoS enemies",
                "TraButton"),
            new LayerInfo(3, "Network", "Man-in-the-Middle", "NetworkLayer",
                "Use WASD to move, mouse to look, Space to jump\nReach Routers 1, 2 and 3, then the destination, while avoiding MITM enemies",
                "NetButton"),
            new LayerInfo(2, "Data Link", "Spoofing", "SpoofingScene",
                "Use AD/Arrow keys to move left and right\nChoose the right door based on the situation",
                "LinButton"),
            new LayerInfo(1, "Physical", "Sniffing", "SniffingScene",
                "Coming soon",
                "PhyButton"),
        };

        public static LayerInfo FindByScene(string sceneName)
        {
            foreach (LayerInfo layer in Layers)
                if (string.Equals(layer.Scene, sceneName, StringComparison.Ordinal))
                    return layer;
            return null;
        }

        public static int IndexOf(LayerInfo layer)
        {
            return Array.IndexOf(Layers, layer);
        }
    }
}
