namespace Evaluation.Dtos
{
    public class FormDtsDto
    {
        public DateTime FormDate { get; set; }

        public string KnowledgeType { get; set; } = null!;

        public int EmpId { get; set; }

        public int FormId { get; set; }

        public int SkillId { get; set; }

        public string SkillDegree { get; set; } = null!;

    }
}
