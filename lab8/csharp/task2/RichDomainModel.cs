namespace Variant3_FitnessMembership.Domain
{
    public class Member
    {
        public int Id { get; }
        public string Name { get; }
        public string Phone { get; }
        public bool HasDiscountCard { get; }

        public Member(int id, string name, string phone, bool hasDiscountCard)
        {
            Id = id;
            Name = name;
            Phone = phone;
            HasDiscountCard = hasDiscountCard;
        }
    }

    public class MembershipPlan
    {
        public string Code { get; } // "Day", "Full", "VIP"
        public string Title { get; }
        public decimal BaseMonthlyRate { get; }
        public int MonthsDuration { get; }

        public MembershipPlan(string code, string title, decimal baseMonthlyRate, int monthsDuration)
        {
            Code = code;
            Title = title;
            BaseMonthlyRate = baseMonthlyRate;
            MonthsDuration = monthsDuration;
        }
    }

    public class Subscription
    {
        public int Id { get; }
        public Member Member { get; }
        public MembershipPlan Plan { get; }
        public bool PersonalTrainerIncluded { get; }
        public decimal BaseAmount { get; private set; }
        public decimal DiscountAmount { get; private set; }
        public decimal FinalCost { get; private set; }
        public string StartDate { get; }
        public string Status { get; private set; }

        // Creator паттерн: конструктор або фабричний метод гарантує створення валідного об'єкта
        public Subscription(int id, Member member, MembershipPlan plan, bool personalTrainerIncluded, string startDate)
        {
            Id = id;
            Member = member;
            Plan = plan;
            PersonalTrainerIncluded = personalTrainerIncluded;
            StartDate = startDate;
            Status = "Draft";

            CalculateCost(); // Information Expert обчислює вартість сам
        }

        // Information Expert: сутність сама знає як рахувати свою вартість
        private void CalculateCost()
        {
            decimal rawCost = Plan.BaseMonthlyRate * Plan.MonthsDuration;

            if (Plan.Code == "VIP")
            {
                rawCost += 5000m; // Персональна шафка та СПА
            }
            else if (Plan.Code == "Day")
            {
                rawCost *= 0.8m; // Знижка на денне відвідування
            }

            if (PersonalTrainerIncluded)
            {
                rawCost += 3000m;
            }

            BaseAmount = rawCost;

            decimal discount = 0m;
            if (Member.HasDiscountCard)
            {
                discount = BaseAmount * 0.10m;
            }

            DiscountAmount = discount;
            FinalCost = BaseAmount - DiscountAmount;
            Status = "Active";
        }

        public string GenerateContract()
        {
            return $"Договір #{Id} з {Member.Name}\n" +
                   $"Тариф: {Plan.Code}\n" +
                   $"Базова вартість: {BaseAmount} грн\n" +
                   $"Знижка: {DiscountAmount} грн\n" +
                   $"До сплати: {FinalCost} грн";
        }
    }
}
