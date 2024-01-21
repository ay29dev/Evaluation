﻿using System;
using System.Collections.Generic;

namespace Evaluation.Models;

public partial class FormMst
{
    public int FormId { get; set; }

    public DateTime FormDate { get; set; }

    public string KnowledgeType { get; set; } = null!;

    public int EmpId { get; set; }
}
