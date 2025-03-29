using ChildApi.Application.DTOs;
using ChildApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ChildApi.Application.Services
{
    public static class GrowthTracker
    {
        // Ngưỡng chuẩn dựa trên WHO (có thể điều chỉnh theo độ tuổi hoặc cấu hình)
        private const decimal UnderweightThreshold = 18.5m;
        private const decimal NormalWeightThreshold = 25m;
        private const decimal OverweightThreshold = 30m;
        private const decimal MinBirthWeight = 2.5m; // kg
        private const decimal MaxBirthWeight = 4.5m; // kg
        private const decimal MinBirthHeight = 45m;  // cm
        private const decimal MaxBirthHeight = 55m;  // cm
        private const decimal WeightHeightRatioMin = 0.10m;
        private const decimal WeightHeightRatioMax = 0.15m;

        // Phân tích sự phát triển và trả về cảnh báo
        public static GrowthAnalysis Analyze(ChildDTO child, DbContext context)
        {
            var analysis = new GrowthAnalysis { ChildId = child.Id ?? Guid.Empty };
            var currentDate = DateTime.Now;

            // Kiểm tra dữ liệu cơ bản
            if (child.BirthWeight == null || child.BirthHeight == null || child.BirthHeight == 0)
            {
                analysis.Warning = "Insufficient data for growth analysis";
                return analysis;
            }

            // Tính BMI
            var bmi = CalculateBMI(child);
            analysis.BMI = bmi;

            // Cảnh báo BMI
            if (bmi < UnderweightThreshold)
                analysis.Warning += "Warning: Child may be underweight | ";
            else if (bmi >= OverweightThreshold)
                analysis.Warning += "Warning: Child may be obese | ";
            else if (bmi >= NormalWeightThreshold)
                analysis.Warning += "Warning: Child may be overweight | ";

            // Cảnh báo cân nặng khi sinh
            if (child.BirthWeight < MinBirthWeight)
                analysis.Warning += "Warning: Birth weight too low (< 2.5kg) | ";
            else if (child.BirthWeight > MaxBirthWeight)
                analysis.Warning += "Warning: Birth weight too high (> 4.5kg) | ";

            // Cảnh báo chiều cao khi sinh
            if (child.BirthHeight < MinBirthHeight)
                analysis.Warning += "Warning: Birth height too low (< 45cm) | ";
            else if (child.BirthHeight > MaxBirthHeight)
                analysis.Warning += "Warning: Birth height too high (> 55cm) | ";

            // Cảnh báo phát triển không đồng đều
            var weightHeightRatio = child.BirthWeight.Value / child.BirthHeight.Value;
            if (weightHeightRatio < WeightHeightRatioMin || weightHeightRatio > WeightHeightRatioMax)
                analysis.Warning += "Warning: Disproportionate growth detected | ";

            // Cảnh báo tuổi phát triển chậm (dựa trên DateOfBirth)
            var ageInMonths = (currentDate - child.DateOfBirth).TotalDays / 30.44; // Ước tính tháng
            if (ageInMonths > 2 && ageInMonths <= 24) // Trẻ từ 2 tháng đến 2 tuổi
            {
                var milestoneCount = context.Set<Milestone>().Count(m => m.ChildId == child.Id && m.MilestoneDate <= currentDate);
                if (milestoneCount == 0)
                    analysis.Warning += "Warning: No milestones recorded, possible delayed development | ";
                else if (milestoneCount < (ageInMonths / 6)) // Ước tính 1 cột mốc mỗi 6 tháng
                    analysis.Warning += "Warning: Insufficient milestones, possible delayed development | ";
            }

            // Xóa ký tự | thừa cuối cùng
            analysis.Warning = analysis.Warning.TrimEnd(' ', '|');
            if (string.IsNullOrEmpty(analysis.Warning))
                analysis.Warning = "No issues detected";

            return analysis;
        }

        private static decimal CalculateBMI(ChildDTO child)
        {
            var heightInMeter = child.BirthHeight.Value / 100;
            return Math.Round(child.BirthWeight.Value / (heightInMeter * heightInMeter), 2);
        }
    }

    public class GrowthAnalysis
    {
        public Guid ChildId { get; set; }
        public decimal? BMI { get; set; }
        public string Warning { get; set; } = string.Empty;
    }
}