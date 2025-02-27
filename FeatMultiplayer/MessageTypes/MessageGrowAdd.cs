﻿using SpaceCraft;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FeatMultiplayer
{
    internal class MessageGrowAdd : MessageStringProvider
    {
        internal int machineId;
        internal int spawnId;
        internal int typeIndex;
        internal float growth;
        internal float growSize;
        internal Vector3 position;
        internal Quaternion rotation;

        internal static bool TryParse(string str, out MessageGrowAdd mga)
        {
            if (MessageHelper.TryParseMessage("GrowAdd|", str, 8, out var parameters))
            {
                try
                {
                    mga = new();
                    mga.machineId = int.Parse(parameters[1]);
                    mga.spawnId = int.Parse(parameters[2]);
                    mga.typeIndex = int.Parse(parameters[3]);
                    mga.growth = float.Parse(parameters[4], CultureInfo.InvariantCulture);
                    mga.growSize = float.Parse(parameters[5], CultureInfo.InvariantCulture);
                    mga.position = DataTreatments.StringToVector3(parameters[6]);
                    mga.rotation = DataTreatments.StringToQuaternion(parameters[7]);
                    return true;
                }
                catch (Exception ex)
                {
                    Plugin.LogError(ex);
                }
            }
            mga = null;
            return false;
        }

        public string GetString()
        {
            return "GrowAdd|" + machineId
                + "|" + spawnId
                + "|" + typeIndex
                + "|" + growth.ToString(CultureInfo.InvariantCulture)
                + "|" + growSize.ToString(CultureInfo.InvariantCulture)
                + "|" + DataTreatments.Vector3ToString(position)
                + "|" + DataTreatments.QuaternionToString(rotation)
                + "\n";
        }
    }
}
