﻿// <auto-generated />
using System;
using Aiursoft.OSS.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Aiursoft.OSS.Migrations
{
    [DbContext(typeof(OSSDbContext))]
    partial class OSSDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Aiursoft.Pylon.Models.OSS.Bucket", b =>
                {
                    b.Property<int>("BucketId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BelongingAppId");

                    b.Property<string>("BucketName");

                    b.Property<bool>("OpenToRead");

                    b.Property<bool>("OpenToUpload");

                    b.HasKey("BucketId");

                    b.HasIndex("BelongingAppId");

                    b.ToTable("Bucket");
                });

            modelBuilder.Entity("Aiursoft.Pylon.Models.OSS.OSSApp", b =>
                {
                    b.Property<string>("AppId")
                        .ValueGeneratedOnAdd();

                    b.HasKey("AppId");

                    b.ToTable("Apps");
                });

            modelBuilder.Entity("Aiursoft.Pylon.Models.OSS.OSSFile", b =>
                {
                    b.Property<int>("FileKey")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AliveDays");

                    b.Property<int>("BucketId");

                    b.Property<int>("DownloadTimes");

                    b.Property<string>("FileExtension");

                    b.Property<string>("RealFileName");

                    b.Property<DateTime>("UploadTime");

                    b.HasKey("FileKey");

                    b.HasIndex("BucketId");

                    b.ToTable("OSSFile");
                });

            modelBuilder.Entity("Aiursoft.Pylon.Models.OSS.Secret", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("FileId");

                    b.Property<DateTime>("UseTime");

                    b.Property<bool>("Used");

                    b.Property<string>("UserIpAddress");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("FileId");

                    b.ToTable("Secrets");
                });

            modelBuilder.Entity("Aiursoft.Pylon.Models.OSS.Bucket", b =>
                {
                    b.HasOne("Aiursoft.Pylon.Models.OSS.OSSApp", "BelongingApp")
                        .WithMany("MyBuckets")
                        .HasForeignKey("BelongingAppId");
                });

            modelBuilder.Entity("Aiursoft.Pylon.Models.OSS.OSSFile", b =>
                {
                    b.HasOne("Aiursoft.Pylon.Models.OSS.Bucket", "BelongingBucket")
                        .WithMany("Files")
                        .HasForeignKey("BucketId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Aiursoft.Pylon.Models.OSS.Secret", b =>
                {
                    b.HasOne("Aiursoft.Pylon.Models.OSS.OSSFile", "File")
                        .WithMany("Secrets")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
