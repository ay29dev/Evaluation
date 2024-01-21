using System;
using System.Collections.Generic;

namespace Evaluation.Models;

public partial class FormDt
{
    public int FormDtsId { get; set; }

    public int FormId { get; set; }

    public int SkillId { get; set; }

    public string SkillDegree { get; set; } = null!;

    public virtual FormMst Form { get; set; } = null!;

    public virtual Skill Skill { get; set; } = null!;
}
