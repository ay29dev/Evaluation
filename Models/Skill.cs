using System;
using System.Collections.Generic;

namespace Evaluation.Models;

public partial class Skill
{
    public int SkillId { get; set; }

    public string SkillType { get; set; } = null!;

    public string SkillTitle { get; set; } = null!;

    public virtual ICollection<FormDt> FormDts { get; set; } = new List<FormDt>();
}
