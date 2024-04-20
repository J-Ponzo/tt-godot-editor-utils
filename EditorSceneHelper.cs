using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TurboTartine.EditorUtils
{
    public static class EditorSceneHelper
    {
        public static PackedScene CreateInherited(this PackedScene baseScene, string rootName = null)
        {
            if (rootName == null) rootName = baseScene.GetState().GetNodeName(0);

            List<string> names = new List<string> { rootName };
            List<Variant> variants = new List<Variant>(new Variant[] { baseScene });
            List<int> nodes = new List<int>(new int[] { -1, -1, 2147483647, 0, -1 });

            SceneState baseScnState = baseScene.GetState();
            int propsCount = baseScnState.GetNodePropertyCount(0);
            nodes.Add(propsCount);
            for (int i = 0; i < propsCount; i++)
            {
                int nameIdx = names.Count;
                names.Add(baseScnState.GetNodePropertyName(0, i));
                nodes.Add(nameIdx);

                int variantIdx = variants.Count;
                variants.Add(baseScnState.GetNodePropertyValue(0, i));
                nodes.Add(variantIdx);
            }

            int grpsCount = baseScnState.GetNodeGroups(0).Length;
            nodes.Add(grpsCount);
            for (int i = 0; i < grpsCount; i++)
            {
                int nameIdx = names.Count;
                names.Add(baseScnState.GetNodeGroups(0)[i]);
                nodes.Add(nameIdx);
            }

            //TODO Setup connections

            PackedScene inheritedScene = new PackedScene();
            Godot.Collections.Dictionary _bundled = inheritedScene._Bundled;
            _bundled["names"] = names.ToArray();
            _bundled["node_count"] = 1;
            _bundled["nodes"] = nodes.ToArray();
            _bundled["variants"] = new Godot.Collections.Array(variants);
            _bundled.Add("base_scene", 0);
            inheritedScene._Bundled = _bundled;

            return inheritedScene;
        }

        public static void PrintBundled(this PackedScene packedScene)
        {
            GD.Print(packedScene._Bundled);

            string names = packedScene._Bundled["names"].ToString();
            GD.Print("Names : " + names);

            string variants = packedScene._Bundled["variants"].ToString();
            GD.Print("Variants : " + variants);

            int idx = 0;
            List<int> nodes = packedScene._Bundled["nodes"].AsInt32Array().ToList<int>();
            while (nodes.Count > 0)
            {
                string nodeStr = "Node " + (idx++) + " : ";
                List<int> node = new List<int>();

                for (int i = 0; i < 5; i++) nodeStr += nodes[i] + "\t";
                node.AddRange(nodes.GetRange(0, 5));
                nodes.RemoveRange(0, 5);

                nodeStr += "|" + "\t";

                int propCount = nodes[0];
                int propRange = 1 + 2 * propCount;
                for (int i = 0; i < propRange; i++) nodeStr += nodes[i] + "\t";
                node.AddRange(nodes.GetRange(0, propRange));
                nodes.RemoveRange(0, propRange);

                nodeStr += "|" + "\t";

                int grpCount = nodes[0];
                int grpRange = 1 + 2 * grpCount;
                for (int i = 0; i < grpRange; i++) nodeStr += nodes[i] + "\t";
                node.AddRange(nodes.GetRange(0, grpRange));
                nodes.RemoveRange(0, grpRange);

                GD.Print(nodeStr);
            }
        }
    }
}
