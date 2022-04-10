using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace CustomerOrder2.Models
{
    public partial class dbOrderContext : DbContext
    {
        public dbOrderContext()
        {
        }

        public dbOrderContext(DbContextOptions<dbOrderContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CustomerInfo> CustomerInfos { get; set; }
        public virtual DbSet<OrderInfo> OrderInfos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Chinese_Taiwan_Stroke_CI_AS");

            modelBuilder.Entity<CustomerInfo>(entity =>
            {
                entity.ToTable("CustomerInfo");

                entity.Property(e => e.Id).HasComment("顧客識別碼");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("寄送地址");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasComment("顧客資料鍵入時間");

                entity.Property(e => e.Mail)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("顧客信箱");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("顧客名稱");

                entity.Property(e => e.UpdateTime)
                    .HasColumnType("datetime")
                    .HasComment("顧客資料修改時間");

                entity.Property(e => e.Visible).HasComment("1代表首頁顯示此筆資料，0代表首頁不顯示此筆資料");
            });

            modelBuilder.Entity<OrderInfo>(entity =>
            {
                entity.HasKey(e => e.OrderId);

                entity.ToTable("OrderInfo");

                entity.Property(e => e.OrderId).HasComment("訂單識別碼");

                entity.Property(e => e.CustomerId).HasComment("顧客識別碼");

                entity.Property(e => e.OrderTime)
                    .HasColumnType("datetime")
                    .HasComment("訂單成立時間");

                entity.Property(e => e.Price).HasComment("訂單總金額");

                entity.Property(e => e.Product1).HasComment("產品一訂購數量");

                entity.Property(e => e.Product2).HasComment("產品二訂購數量");

                entity.Property(e => e.Visible).HasComment("1代表首頁顯示此筆資料，0代表首頁不顯示此筆資料");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
