using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace WebBanHang.Models
{
    public partial class dbBanHangContext : DbContext
    {
        public dbBanHangContext()
        {
        }

        public dbBanHangContext(DbContextOptions<dbBanHangContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public virtual DbSet<DanhGiaSanPham> DanhGiaSanPhams { get; set; }
        public virtual DbSet<DonHang> DonHangs { get; set; }
        public virtual DbSet<KhachHang> KhachHangs { get; set; }
        public virtual DbSet<KhuyenMai> KhuyenMais { get; set; }
        public virtual DbSet<LoaiSanPham> LoaiSanPhams { get; set; }
        public virtual DbSet<QuanHuyen> QuanHuyens { get; set; }
        public virtual DbSet<QuanTriVien> QuanTriViens { get; set; }
        public virtual DbSet<SanPham> SanPhams { get; set; }
        public virtual DbSet<Shipper> Shippers { get; set; }
        public virtual DbSet<ThuongHieu> ThuongHieus { get; set; }
        public virtual DbSet<TinhThanhPho> TinhThanhPhos { get; set; }
        public virtual DbSet<TrangThaiDonHang> TrangThaiDonHangs { get; set; }
        public virtual DbSet<XaPhuongThiTran> XaPhuongThiTrans { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=LAPTOP-73R0Q1AI\\SQLEXPRESS01;Initial Catalog=dbBanHang;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<ChiTietDonHang>(entity =>
            {
                entity.HasKey(e => new { e.MaDh, e.MaSp });

                entity.ToTable("ChiTietDonHang");

                entity.Property(e => e.MaDh).HasColumnName("MaDH");

                entity.Property(e => e.MaSp).HasColumnName("MaSP");

                entity.HasOne(d => d.MaDhNavigation)
                    .WithMany(p => p.ChiTietDonHangs)
                    .HasForeignKey(d => d.MaDh)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChiTietDonHang_DonHang");

                entity.HasOne(d => d.MaSpNavigation)
                    .WithMany(p => p.ChiTietDonHangs)
                    .HasForeignKey(d => d.MaSp)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChiTietDonHang_SanPham");
            });

            modelBuilder.Entity<DanhGiaSanPham>(entity =>
            {
                entity.HasKey(e => e.MaDg);

                entity.ToTable("DanhGiaSanPham");

                entity.Property(e => e.MaDg).HasColumnName("MaDG");

                entity.Property(e => e.MaKh).HasColumnName("MaKH");

                entity.Property(e => e.MaSp).HasColumnName("MaSP");

                entity.Property(e => e.NoiDung).HasMaxLength(500);

                entity.Property(e => e.ThoiGian).HasColumnType("datetime");

                entity.HasOne(d => d.MaKhNavigation)
                    .WithMany(p => p.DanhGiaSanPhams)
                    .HasForeignKey(d => d.MaKh)
                    .HasConstraintName("FK_DanhGiaSanPham_KhachHang");

                entity.HasOne(d => d.MaSpNavigation)
                    .WithMany(p => p.DanhGiaSanPhams)
                    .HasForeignKey(d => d.MaSp)
                    .HasConstraintName("FK_DanhGiaSanPham_SanPham");
            });

            modelBuilder.Entity<DonHang>(entity =>
            {
                entity.HasKey(e => e.MaDh);

                entity.ToTable("DonHang");

                entity.Property(e => e.MaDh).HasColumnName("MaDH");

                entity.Property(e => e.DiaChi).HasMaxLength(500);

                entity.Property(e => e.HoTen).HasMaxLength(250);

                entity.Property(e => e.MaKh).HasColumnName("MaKH");

                entity.Property(e => e.MaKm).HasColumnName("MaKM");

                entity.Property(e => e.MaTt).HasColumnName("MaTT");

                entity.Property(e => e.Maqh)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.Matp)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.Maxa)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.NgayDat).HasColumnType("datetime");

                entity.Property(e => e.NgayShip).HasColumnType("datetime");

                entity.Property(e => e.PhuongThucThanhToan).HasMaxLength(250);

                entity.Property(e => e.Sdt)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SDT")
                    .IsFixedLength(true);

                entity.HasOne(d => d.MaKhNavigation)
                    .WithMany(p => p.DonHangs)
                    .HasForeignKey(d => d.MaKh)
                    .HasConstraintName("FK_DonHang_KhachHang");

                entity.HasOne(d => d.MaKmNavigation)
                    .WithMany(p => p.DonHangs)
                    .HasForeignKey(d => d.MaKm)
                    .HasConstraintName("FK_DonHang_KhuyenMai");

                entity.HasOne(d => d.MaShipperNavigation)
                    .WithMany(p => p.DonHangs)
                    .HasForeignKey(d => d.MaShipper)
                    .HasConstraintName("FK_DonHang_Shipper");

                entity.HasOne(d => d.MaTtNavigation)
                    .WithMany(p => p.DonHangs)
                    .HasForeignKey(d => d.MaTt)
                    .HasConstraintName("FK_DonHang_TrangThaiDonHang");
            });

            modelBuilder.Entity<KhachHang>(entity =>
            {
                entity.HasKey(e => e.MaKh);

                entity.ToTable("KhachHang");

                entity.Property(e => e.MaKh).HasColumnName("MaKH");

                entity.Property(e => e.DiaChi).HasMaxLength(500);

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Maqh)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.MatKhau).HasMaxLength(50);

                entity.Property(e => e.Matp)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.Maxa)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.Salt).HasMaxLength(50);

                entity.Property(e => e.Sdt)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SDT")
                    .IsFixedLength(true);

                entity.Property(e => e.TenKh)
                    .HasMaxLength(250)
                    .HasColumnName("TenKH");
            });

            modelBuilder.Entity<KhuyenMai>(entity =>
            {
                entity.HasKey(e => e.MaKm);

                entity.ToTable("KhuyenMai");

                entity.Property(e => e.MaKm).HasColumnName("MaKM");

                entity.Property(e => e.MaNhap).HasMaxLength(50);

                entity.Property(e => e.MoTa).HasMaxLength(250);

                entity.Property(e => e.NgayBatDau).HasColumnType("date");

                entity.Property(e => e.NgayKetThuc).HasColumnType("date");
            });

            modelBuilder.Entity<LoaiSanPham>(entity =>
            {
                entity.HasKey(e => e.MaLoai)
                    .HasName("PK_LOAISANPHAM");

                entity.ToTable("LoaiSanPham");

                entity.Property(e => e.MoTa).HasMaxLength(250);

                entity.Property(e => e.TenLoai).HasMaxLength(250);
            });

            modelBuilder.Entity<QuanHuyen>(entity =>
            {
                entity.HasKey(e => e.Maqh)
                    .HasName("PK__QuanHuye__2724F3DEBB717CB8");

                entity.ToTable("QuanHuyen");

                entity.Property(e => e.Maqh)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.Matp)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.HasOne(d => d.MatpNavigation)
                    .WithMany(p => p.QuanHuyens)
                    .HasForeignKey(d => d.Matp)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuanHuyen_TinhThanhPho");
            });

            modelBuilder.Entity<QuanTriVien>(entity =>
            {
                entity.HasKey(e => e.MaQtv);

                entity.ToTable("QuanTriVien");

                entity.Property(e => e.MaQtv).HasColumnName("MaQTV");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.MatKhau).HasMaxLength(50);

                entity.Property(e => e.Salt).HasMaxLength(50);

                entity.Property(e => e.Sdt)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SDT")
                    .IsFixedLength(true);

                entity.Property(e => e.TenQtv)
                    .HasMaxLength(250)
                    .HasColumnName("TenQTV");
            });

            modelBuilder.Entity<SanPham>(entity =>
            {
                entity.HasKey(e => e.MaSp);

                entity.ToTable("SanPham");

                entity.Property(e => e.MaSp).HasColumnName("MaSP");

                entity.Property(e => e.Anh).HasMaxLength(250);

                entity.Property(e => e.BaoHanh).HasMaxLength(50);

                entity.Property(e => e.MaTh).HasColumnName("MaTH");

                entity.Property(e => e.MoTa).HasMaxLength(500);

                entity.Property(e => e.TenSp)
                    .HasMaxLength(250)
                    .HasColumnName("TenSP");

                entity.HasOne(d => d.MaLoaiNavigation)
                    .WithMany(p => p.SanPhams)
                    .HasForeignKey(d => d.MaLoai)
                    .HasConstraintName("FK_SanPham_LoaiSanPham");

                entity.HasOne(d => d.MaThNavigation)
                    .WithMany(p => p.SanPhams)
                    .HasForeignKey(d => d.MaTh)
                    .HasConstraintName("FK_SanPham_ThuongHieu");
            });

            modelBuilder.Entity<Shipper>(entity =>
            {
                entity.HasKey(e => e.MaShipper);

                entity.ToTable("Shipper");

                entity.Property(e => e.BienSo).HasMaxLength(12);

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.LoaiXe).HasMaxLength(50);

                entity.Property(e => e.MatKhau).HasMaxLength(50);

                entity.Property(e => e.Salt).HasMaxLength(50);

                entity.Property(e => e.Sdt)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SDT")
                    .IsFixedLength(true);

                entity.Property(e => e.TenHt)
                    .HasMaxLength(500)
                    .HasColumnName("TenHT");

                entity.Property(e => e.TenShipper).HasMaxLength(250);
            });

            modelBuilder.Entity<ThuongHieu>(entity =>
            {
                entity.HasKey(e => e.MaTh);

                entity.ToTable("ThuongHieu");

                entity.Property(e => e.MaTh).HasColumnName("MaTH");

                entity.Property(e => e.MoTa).HasMaxLength(250);

                entity.Property(e => e.TenTh)
                    .HasMaxLength(250)
                    .HasColumnName("TenTH");
            });

            modelBuilder.Entity<TinhThanhPho>(entity =>
            {
                entity.HasKey(e => e.Matp)
                    .HasName("PK__TinhThan__2724FBC5B42250B7");

                entity.ToTable("TinhThanhPho");

                entity.Property(e => e.Matp)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Slug)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<TrangThaiDonHang>(entity =>
            {
                entity.HasKey(e => e.MaTt);

                entity.ToTable("TrangThaiDonHang");

                entity.Property(e => e.MaTt)
                    .ValueGeneratedNever()
                    .HasColumnName("MaTT");

                entity.Property(e => e.MoTa).HasMaxLength(250);

                entity.Property(e => e.TenTt)
                    .HasMaxLength(100)
                    .HasColumnName("TenTT");
            });

            modelBuilder.Entity<XaPhuongThiTran>(entity =>
            {
                entity.HasKey(e => e.Maxa)
                    .HasName("PK__XaPhuong__B9169A76FDCF96AF");

                entity.ToTable("XaPhuongThiTran");

                entity.Property(e => e.Maxa)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.Maqh)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.HasOne(d => d.MaqhNavigation)
                    .WithMany(p => p.XaPhuongThiTrans)
                    .HasForeignKey(d => d.Maqh)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_XaPhuongThiTran_QuanHuyen");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
