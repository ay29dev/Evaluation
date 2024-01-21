using System;
using System.Collections.Generic;

namespace Evaluation.Models;

public partial class Employee
{
    public int EmpId { get; set; }

    public string EmpName { get; set; } = null!;

    public string EmpTitle { get; set; } = null!;

    public string EmpStep { get; set; } = null!;

    public string EmpDep { get; set; } = null!;
}
