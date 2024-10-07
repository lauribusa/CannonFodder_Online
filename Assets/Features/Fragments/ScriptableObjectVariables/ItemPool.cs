using Assets.Features.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Features.Fragments.ScriptableObjectVariables
{
    [CreateAssetMenu(menuName = "Pool/Item")]
    public class ItemPool: PoolSO<Item>
    {
    }
}
