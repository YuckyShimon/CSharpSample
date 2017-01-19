using System;
using System.Data.Linq.Mapping;
using System.Diagnostics.CodeAnalysis;

namespace CSharpSample.SampleCode.Entity
{
    /// <summary>
    /// 家計簿クラスのエンティティ
    /// TableアトリビュートにはSystem.Data.Linqへの参照が必要
    /// </summary>
    [Table(Name = "dbo.HouseholdAccount")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "For Japanese support")]
    public class HouseholdAccount
    {
        /// <summary>
        /// レコードId
        /// </summary>
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "int NOT NULL IDENTITY")]
        public int Id { get; set; }

        /// <summary>
        /// 日付
        /// </summary>
        [Column]
        public DateTime Date { get; set; }

        /// <summary>
        /// 品目
        /// </summary>
        [Column(CanBeNull = true)]
        public string Item { get; set; }

        /// <summary>
        /// メモ
        /// </summary>
        [Column(CanBeNull = true)]
        public string Memo { get; set; }

        /// <summary>
        /// 入金
        /// </summary>
        [Column(CanBeNull = true)]
        public int? Payment { get; set; }

        /// <summary>
        /// 支払い
        /// </summary>
        [Column(CanBeNull = true)]
        public int? Withdrawal { get; set; }
    }
}
