using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public interface IHasSkill 
{
    List<SkillSO> SkillListSO { get;  }
}
