using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class Contract
    {
        [Key]
        public long ID { get; set; }
        public long WorkPlanPackageId { get; set; }
        public WorkPlanPackage WorkPlanPackage { get; set; }
        [Display(Name ="% Retention")]
        public float PercentageRetention { get; set; }
        [Display(Name ="Performance Bond %")]
        public float PerformanceBond { get; set; }
        [Display(Name ="Advance Payment (KSH)")]
        public double AdvancePayment { get; set; }
        [Display(Name ="Contract Sum (WorkPlan Cost)")]
        public double ContractSumWorkplan { get; set; }

        [Display(Name = "Contract Sum (Engineer Estimate)")]
        public double ContractSumPackage { get; set; }
        [Display(Name ="Advance Clearance (KSH)")]
        public double AdvanceClearance { get; set; }
        [Display(Name = "In Payment Certificate")]
        public bool inPaymentCertificate { get; set; }
        [Display(Name ="Contract Not Taxable")]
        public bool ContractTaxable { get; set; }
        [Display(Name ="Contract Signed")]
        public bool ContractIsSigned { get; set; }

        //[DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}")]
        [DisplayFormat(DataFormatString = "{0: MM/dd/yyyy}")]
        [Display(Name = "Contract Start Date")]
        public DateTime? ContractStartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0: MM/dd/yyyy}")]
        [Display(Name ="Contract End Date")]
        public DateTime? ContractEndDate { get; set; }
        [Display(Name = "Special Group")]
        public int SpecialGroup { get; set; }
        public long? ContractorId { get; set; }
        public Contractor Contractor { get; set; }
        public string UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public int Status { get; set; }

        //project brief data
        [Display(Name = "Project Title")]
        public string ProjectTitle { get; set; }
        [Display(Name = "Period Month")]
        public int PeriodMonth { get; set; }
        public string Financier { get; set; }
        public string Employer { get; set; }
        public string Engineer { get; set; }
        [Display(Name = "Engineer Representative")]
        public string EngineerRepresentative { get; set; }
        [Display(Name = "Contract Sum (KSH)")]
        public double ContractSum { get; set; }
        public float percentageOfWorkDone { get; set; }
        [Display(Name = "Amount Certified to Date (KSH)")]
        public double AmountCertifiedToDate { get; set; }
        public float percentageOfCertifiedPermanentWork { get; set; }
        public int ExtensionGrantedMonths { get; set; }
        [DisplayFormat(DataFormatString = "{0: MM/dd/yyyy}")]
        [Display(Name = "Date of Tender")]
        public DateTime? TenderDate { get; set; }
        [DisplayFormat(DataFormatString = "{0: MM/dd/yyyy}")]
        [Display(Name = "Date of Award")]
        public DateTime? AwardDate { get; set; }
        [DisplayFormat(DataFormatString = "{0: MM/dd/yyyy}")]
        [Display(Name = "Commencement Date")]
        public DateTime? CommencementDate { get; set; }
        [DisplayFormat(DataFormatString = "{0: MM/dd/yyyy}")]
        [Display(Name = "Date of Completion")]
        public DateTime? CompletionDate { get; set; }

        [DisplayFormat(DataFormatString = "{0: MM/dd/yyyy}")]
        [Display(Name = "Date of Actual Completion")]
        public DateTime? ActualCompletionDate { get; set; }


        [DisplayFormat(DataFormatString = "{0: MM/dd/yyyy}")]
        [Display(Name = "Previous Certificate Date")]
        public DateTime? PreviousCertificateDate { get; set; }
        [DisplayFormat(DataFormatString = "{0: MM/dd/yyyy}")]
        [Display(Name = "Current Certificate Date")]
        public DateTime? LatestCertificateDate { get; set; }

        public double ContractRevisionSum { get; set; }
        public string RevisionStatus { get; set; }

        public string revisionRemarks { get; set; }
        [Display(Name = "Male Count")]
        public int MaleCount { get; set; }
        [Display(Name = "Female Count")]
        public int FemaleCount { get; set; }
        [Display(Name = "Male Person days")]
        public int MalePersonDays { get; set; }
        [Display(Name = "Female Person Days")]
        public int FeMalePersonDays { get; set; }
        [Display(Name = "Remaining Target Man Days")]
        public int RemainingTargetManDays { get; set; }
        public IEnumerable<PaymentCertificate> PaymentCertificates { get;}
        public virtual IEnumerable<EmploymentDetail> EmploymentDetails { get; }
        public int CurrentCertificateNo { get; set; }

    }
}
