using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PagAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CriarContratosEParcelas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IgnorarDomingos",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "IgnorarFeriados",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "IgnorarSabados",
                table: "Contratos");

            migrationBuilder.RenameColumn(
                name: "ValorTotal",
                table: "Contratos",
                newName: "ValorTotalComJuros");

            migrationBuilder.RenameColumn(
                name: "ValorJuros",
                table: "Contratos",
                newName: "ValorEmprestado");

            migrationBuilder.RenameColumn(
                name: "ValorAtraso",
                table: "Contratos",
                newName: "TaxaMulta");

            migrationBuilder.RenameColumn(
                name: "TipoCobrancaAtraso",
                table: "Contratos",
                newName: "TipoMulta");

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Parcelas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataCadastro",
                table: "Parcelas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Observacao",
                table: "Parcelas",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorPago",
                table: "Parcelas",
                type: "numeric",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "QuantidadeParcelas",
                table: "Contratos",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Periodicidade",
                table: "Contratos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PrimeiroVencimento",
                table: "Contratos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Contratos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxaJuros",
                table: "Contratos",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Titulo",
                table: "Contratos",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Parcelas");

            migrationBuilder.DropColumn(
                name: "DataCadastro",
                table: "Parcelas");

            migrationBuilder.DropColumn(
                name: "Observacao",
                table: "Parcelas");

            migrationBuilder.DropColumn(
                name: "ValorPago",
                table: "Parcelas");

            migrationBuilder.DropColumn(
                name: "Periodicidade",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "PrimeiroVencimento",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "TaxaJuros",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "Titulo",
                table: "Contratos");

            migrationBuilder.RenameColumn(
                name: "ValorTotalComJuros",
                table: "Contratos",
                newName: "ValorTotal");

            migrationBuilder.RenameColumn(
                name: "ValorEmprestado",
                table: "Contratos",
                newName: "ValorJuros");

            migrationBuilder.RenameColumn(
                name: "TipoMulta",
                table: "Contratos",
                newName: "TipoCobrancaAtraso");

            migrationBuilder.RenameColumn(
                name: "TaxaMulta",
                table: "Contratos",
                newName: "ValorAtraso");

            migrationBuilder.AlterColumn<int>(
                name: "QuantidadeParcelas",
                table: "Contratos",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<bool>(
                name: "IgnorarDomingos",
                table: "Contratos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IgnorarFeriados",
                table: "Contratos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IgnorarSabados",
                table: "Contratos",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
