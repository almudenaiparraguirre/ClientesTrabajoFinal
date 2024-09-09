using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ApiBasesDeDatosProyecto.Migrations
{
    /// <inheritdoc />
    public partial class paises : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessMonitoringDatas",
                columns: table => new
                {
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Empleo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaisId = table.Column<int>(type: "int", nullable: false),
                    Pais = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Usuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoAcceso = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaRecibido = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessMonitoringDatas", x => x.Email);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Rol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonitoringDatas",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PaisOrigen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaisDestino = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClienteOrigen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClienteDestino = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValorOrigen = table.Column<double>(type: "float", nullable: false),
                    ValorDestino = table.Column<double>(type: "float", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitoringDatas", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Paises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Divisa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Iso = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paises", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProAlmClientePorPaisDtos",
                columns: table => new
                {
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteNombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClienteApellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Empleo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaisId = table.Column<int>(type: "int", nullable: false),
                    PaisNombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Divisa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Iso3 = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProAlmClientePorPaisDtos", x => x.ClienteId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    dateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Empleo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaisId = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clientes_Paises_PaisId",
                        column: x => x.PaisId,
                        principalTable: "Paises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Paises",
                columns: new[] { "Id", "Divisa", "Iso", "Nombre" },
                values: new object[,]
                {
                    { 1, "EUR", "AD", "Andorra" },
                    { 2, "AED", "AE", "United Arab Emirates" },
                    { 3, "AFN", "AF", "Afghanistan" },
                    { 4, "XCD", "AG", "Antigua and Barbuda" },
                    { 5, "XCD", "AI", "Anguilla" },
                    { 6, "ALL", "AL", "Albania" },
                    { 7, "AMD", "AM", "Armenia" },
                    { 8, "AOA", "AO", "Angola" },
                    { 9, "AND", "AQ", "Antarctica" },
                    { 10, "ARS", "AR", "Argentina" },
                    { 11, "EUR", "BL", "Saint Barthélemy" },
                    { 12, "BMD", "BM", "Bermuda" },
                    { 13, "BND", "BN", "Brunei" },
                    { 14, "BOB", "BO", "Bolivia" },
                    { 15, "USD", "BQ", "Bonaire" },
                    { 16, "BRL", "BR", "Brazil" },
                    { 17, "BSD", "BS", "Bahamas" },
                    { 18, "BTN", "BT", "Bhutan" },
                    { 19, "NOK", "BV", "Bouvet Island" },
                    { 20, "BWP", "BW", "Botswana" },
                    { 21, "BYR", "BY", "Belarus" },
                    { 22, "BZD", "BZ", "Belize" },
                    { 23, "CAD", "CA", "Canada" },
                    { 24, "AUD", "CC", "Cocos [Keeling] Islands" },
                    { 25, "CDF", "CD", "Democratic Republic of the Congo" },
                    { 26, "XAF", "CF", "Central African Republic" },
                    { 27, "XAF", "CG", "Republic of the Congo" },
                    { 28, "CHF", "CH", "Switzerland" },
                    { 29, "XOF", "CI", "Ivory Coast" },
                    { 30, "NZD", "CK", "Cook Islands" },
                    { 31, "CLP", "CL", "Chile" },
                    { 32, "XAF", "CM", "Cameroon" },
                    { 33, "CNY", "CN", "China" },
                    { 34, "COP", "CO", "Colombia" },
                    { 35, "CRC", "CR", "Costa Rica" },
                    { 36, "CUP", "CU", "Cuba" },
                    { 37, "CVE", "CV", "Cape Verde" },
                    { 38, "ANG", "CW", "Curacao" },
                    { 39, "AUD", "CX", "Christmas Island" },
                    { 40, "EUR", "CY", "Cyprus" },
                    { 41, "CZK", "CZ", "Czechia" },
                    { 42, "EUR", "DE", "Germany" },
                    { 43, "DJF", "DJ", "Djibouti" },
                    { 44, "DKK", "DK", "Denmark" },
                    { 45, "XCD", "DM", "Dominica" },
                    { 46, "DOP", "DO", "Dominican Republic" },
                    { 47, "DZD", "DZ", "Algeria" },
                    { 48, "USD", "EC", "Ecuador" },
                    { 49, "EUR", "EE", "Estonia" },
                    { 50, "EGP", "EG", "Egypt" },
                    { 51, "MAD", "EH", "Western Sahara" },
                    { 52, "ERN", "ER", "Eritrea" },
                    { 53, "EUR", "ES", "Spain" },
                    { 54, "ETB", "ET", "Ethiopia" },
                    { 55, "EUR", "FI", "Finland" },
                    { 56, "FJD", "FJ", "Fiji" },
                    { 57, "FKP", "FK", "Falkland Islands" },
                    { 58, "USD", "FM", "Micronesia" },
                    { 59, "DKK", "FO", "Faroe Islands" },
                    { 60, "EUR", "FR", "France" },
                    { 61, "XAF", "GA", "Gabon" },
                    { 62, "GBP", "GB", "United Kingdom" },
                    { 63, "XCD", "GD", "Grenada" },
                    { 64, "GEL", "GE", "Georgia" },
                    { 65, "EUR", "GF", "French Guiana" },
                    { 66, "GBP", "GG", "Guernsey" },
                    { 67, "GHS", "GH", "Ghana" },
                    { 68, "GIP", "GI", "Gibraltar" },
                    { 69, "DKK", "GL", "Greenland" },
                    { 70, "GMD", "GM", "Gambia" },
                    { 71, "GNF", "GN", "Guinea" },
                    { 72, "EUR", "GP", "Guadeloupe" },
                    { 73, "XAF", "GQ", "Equatorial Guinea" },
                    { 74, "EUR", "GR", "Greece" },
                    { 75, "GBP", "GS", "South Georgia and the South Sandwich Islands" },
                    { 76, "GTQ", "GT", "Guatemala" },
                    { 77, "USD", "GU", "Guam" },
                    { 78, "GYD", "GY", "Guyana" },
                    { 79, "HKD", "HK", "Hong Kong" },
                    { 80, "HNL", "HN", "Honduras" },
                    { 81, "HRK", "HR", "Croatia" },
                    { 82, "HTG", "HT", "Haiti" },
                    { 83, "HUF", "HU", "Hungary" },
                    { 84, "IDR", "ID", "Indonesia" },
                    { 85, "EUR", "IE", "Ireland" },
                    { 86, "ILS", "IL", "Israel" },
                    { 87, "GBP", "IM", "Isle of Man" },
                    { 88, "INR", "IN", "India" },
                    { 89, "USD", "IO", "British Indian Ocean Territory" },
                    { 90, "IQD", "IQ", "Iraq" },
                    { 91, "IRR", "IR", "Iran" },
                    { 92, "ISK", "IS", "Iceland" },
                    { 93, "EUR", "IT", "Italy" },
                    { 94, "GBP", "JE", "Jersey" },
                    { 95, "JMD", "JM", "Jamaica" },
                    { 96, "JOD", "JO", "Jordan" },
                    { 97, "JPY", "JP", "Japan" },
                    { 98, "KES", "KE", "Kenya" },
                    { 99, "KGS", "KG", "Kyrgyzstan" },
                    { 100, "KHR", "KH", "Cambodia" },
                    { 101, "AUD", "KI", "Kiribati" },
                    { 102, "KMF", "KM", "Comoros" },
                    { 103, "XCD", "KN", "Saint Kitts and Nevis" },
                    { 104, "KPW", "KP", "North Korea" },
                    { 105, "KRW", "KR", "South Korea" },
                    { 106, "KWD", "KW", "Kuwait" },
                    { 107, "KYD", "KY", "Cayman Islands" },
                    { 108, "KZT", "KZ", "Kazakhstan" },
                    { 109, "LAK", "LA", "Laos" },
                    { 110, "LBP", "LB", "Lebanon" },
                    { 111, "XCD", "LC", "Saint Lucia" },
                    { 112, "CHF", "LI", "Liechtenstein" },
                    { 113, "LKR", "LK", "Sri Lanka" },
                    { 114, "LRD", "LR", "Liberia" },
                    { 115, "LSL", "LS", "Lesotho" },
                    { 116, "EUR", "LT", "Lithuania" },
                    { 117, "EUR", "LU", "Luxembourg" },
                    { 118, "EUR", "LV", "Latvia" },
                    { 119, "MAD", "MA", "Morocco" },
                    { 120, "EUR", "MC", "Monaco" },
                    { 121, "MDL", "MD", "Moldova" },
                    { 122, "EUR", "ME", "Montenegro" },
                    { 123, "EUR", "MF", "Saint Martin" },
                    { 124, "MGA", "MG", "Madagascar" },
                    { 125, "USD", "MH", "Marshall Islands" },
                    { 126, "MKD", "MK", "North Macedonia" },
                    { 127, "XOF", "ML", "Mali" },
                    { 128, "MMK", "MM", "Myanmar" },
                    { 129, "MNT", "MN", "Mongolia" },
                    { 130, "MOP", "MO", "Macao" },
                    { 131, "USD", "MP", "Northern Mariana Islands" },
                    { 132, "EUR", "MQ", "Martinique" },
                    { 133, "MRU", "MR", "Mauritania" },
                    { 134, "XCD", "MS", "Montserrat" },
                    { 135, "EUR", "MT", "Malta" },
                    { 136, "MUR", "MU", "Mauritius" },
                    { 137, "MWK", "MW", "Malawi" },
                    { 138, "MXN", "MX", "Mexico" },
                    { 139, "MYR", "MY", "Malaysia" },
                    { 140, "MZN", "MZ", "Mozambique" },
                    { 141, "NAD", "NA", "Namibia" },
                    { 142, "XPF", "NC", "New Caledonia" },
                    { 143, "XOF", "NE", "Niger" },
                    { 144, "AUD", "NF", "Norfolk Island" },
                    { 145, "NGN", "NG", "Nigeria" },
                    { 146, "NIO", "NI", "Nicaragua" },
                    { 147, "EUR", "NL", "Netherlands" },
                    { 148, "NOK", "NO", "Norway" },
                    { 149, "NPR", "NP", "Nepal" },
                    { 150, "AUD", "NR", "Nauru" },
                    { 151, "NZD", "NU", "Niue" },
                    { 152, "NZD", "NZ", "New Zealand" },
                    { 153, "OMR", "OM", "Oman" },
                    { 154, "PAB", "PA", "Panama" },
                    { 155, "PEN", "PE", "Peru" },
                    { 156, "XPF", "PF", "French Polynesia" },
                    { 157, "PGK", "PG", "Papua New Guinea" },
                    { 158, "PHP", "PH", "Philippines" },
                    { 159, "PKR", "PK", "Pakistan" },
                    { 160, "PLN", "PL", "Poland" },
                    { 161, "EUR", "PM", "Saint Pierre and Miquelon" },
                    { 162, "NZD", "PN", "Pitcairn Islands" },
                    { 163, "EUR", "PT", "Portugal" },
                    { 164, "ILS", "PS", "Palestine" },
                    { 165, "USD", "PR", "Puerto Rico" },
                    { 166, "QAR", "QA", "Qatar" },
                    { 167, "EUR", "RE", "Réunion" },
                    { 168, "RON", "RO", "Romania" },
                    { 169, "RUB", "RU", "Russia" },
                    { 170, "RWF", "RW", "Rwanda" },
                    { 171, "SAR", "SA", "Saudi Arabia" },
                    { 172, "SBD", "SB", "Solomon Islands" },
                    { 173, "SCR", "SC", "Seychelles" },
                    { 174, "SDG", "SD", "Sudan" },
                    { 175, "SEK", "SE", "Sweden" },
                    { 176, "SGD", "SG", "Singapore" },
                    { 177, "SHP", "SH", "Saint Helena" },
                    { 178, "EUR", "SI", "Slovenia" },
                    { 179, "NOK", "SJ", "Svalbard and Jan Mayen" },
                    { 180, "EUR", "SK", "Slovakia" },
                    { 181, "SLL", "SL", "Sierra Leone" },
                    { 182, "EUR", "SM", "San Marino" },
                    { 183, "SOS", "SO", "Somalia" },
                    { 184, "SRD", "SR", "Suriname" },
                    { 185, "SSP", "SS", "South Sudan" },
                    { 186, "STN", "ST", "Sao Tome and Principe" },
                    { 187, "USD", "SV", "El Salvador" },
                    { 188, "ANG", "SX", "Sint Maarten" },
                    { 189, "SYP", "SY", "Syria" },
                    { 190, "SZL", "SZ", "Eswatini" },
                    { 191, "USD", "TC", "Turks and Caicos Islands" },
                    { 192, "XAF", "TD", "Chad" },
                    { 193, "EUR", "TF", "French Southern Territories" },
                    { 194, "XOF", "TG", "Togo" },
                    { 195, "THB", "TH", "Thailand" },
                    { 196, "USD", "TL", "Timor-Leste" },
                    { 197, "TOP", "TO", "Tonga" },
                    { 198, "TTD", "TT", "Trinidad and Tobago" },
                    { 199, "TND", "TN", "Tunisia" },
                    { 200, "TRY", "TR", "Turkey" },
                    { 201, "TWD", "TW", "Taiwan" },
                    { 202, "TZS", "TZ", "Tanzania" },
                    { 203, "UAH", "UA", "Ukraine" },
                    { 204, "UGX", "UG", "Uganda" },
                    { 205, "USD", "UM", "United States Minor Outlying Islands" },
                    { 206, "USD", "US", "United States" },
                    { 207, "UYU", "UY", "Uruguay" },
                    { 208, "UZS", "UZ", "Uzbekistan" },
                    { 209, "EUR", "VA", "Vatican City" },
                    { 210, "XCD", "VC", "Saint Vincent and the Grenadines" },
                    { 211, "VES", "VE", "Venezuela" },
                    { 212, "USD", "VG", "British Virgin Islands" },
                    { 213, "USD", "VI", "United States Virgin Islands" },
                    { 214, "VND", "VN", "Vietnam" },
                    { 215, "VUV", "VU", "Vanuatu" },
                    { 216, "XPF", "WF", "Wallis and Futuna" },
                    { 217, "WST", "WS", "Samoa" },
                    { 218, "YER", "YE", "Yemen" },
                    { 219, "ZAR", "ZA", "South Africa" },
                    { 220, "ZMW", "ZM", "Zambia" },
                    { 221, "ZWL", "ZW", "Zimbabwe" }
                });

            migrationBuilder.InsertData(
                table: "Clientes",
                columns: new[] { "Id", "Apellido", "Email", "Empleo", "Nombre", "PaisId", "dateOfBirth" },
                values: new object[,]
                {
                    { 1, "Perez", "amin1@gmail.com", "Delincuente", "Juan", 1, new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "Lopez", "amin2@gmail.com", "Profesor", "Maria", 2, new DateTime(1985, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "Gomez", "amin3@gmail.com", "Abogado", "Carlos", 3, new DateTime(1978, 11, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_PaisId",
                table: "Clientes",
                column: "PaisId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessMonitoringDatas");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "MonitoringDatas");

            migrationBuilder.DropTable(
                name: "ProAlmClientePorPaisDtos");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Paises");
        }
    }
}
