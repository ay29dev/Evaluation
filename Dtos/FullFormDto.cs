namespace Evaluation.Dtos
{
    public class FullFormDto
    {
        public int FormId { get; set; }

        public DateTime FormDate { get; set; }

        public string KnowledgeType { get; set; } = null!;

        public int EmpId { get; set; }

        public int SkillId { get; set; }

        public string SkillDegree { get; set; } = null!;

    }
}
