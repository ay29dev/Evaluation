namespace Evaluation.Dtos
{
    public class FormMstDto
    {
        public int FormId { get; set; }

        public DateTime FormDate { get; set; }

        public string KnowledgeType { get; set; } = null!;

        public int EmpId { get; set; }

    }
}
