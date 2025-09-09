# 📦 Kargo Teslim Web Uygulaması

**Problem:** Yurtlarda kargolar doğrudan içeri alınmadığı için öğrenciler kargolarını teslim almakta zorluk yaşıyor.  
**Çözüm:** Bu proje, öğrencilerin kargolarını **güvenilir kişiler aracılığıyla** teslim alabilmesini sağlayan bir **ilan, takip ve güven sistemi** sunar.  

---

## 🚀 Özellikler
- Kullanıcı kayıt & giriş (JWT Authentication)  
- Kargo teslim ilanı açma ve listeleme  
- **Kargo takip ekranı** (aktif kargoların durumunu görme)  
- **Trust Score**: kullanıcıların güven puanı metriği  
- Güvenilir teslim edicilerin önerilmesi (puanlama sistemi)  
- Kullanıcı profilinde güven puanı ve ilan geçmişi  

---

## 🛠 Teknolojiler
- **Backend:** .NET 6, ASP.NET Core, Entity Framework Core, MSSQL, JWT, Swagger  
- **Frontend:** Angular, TypeScript, Bootstrap  

---

## ⚙️ Çalıştırma
```bash
# Backend
dotnet restore
dotnet ef database update
dotnet run   # http://localhost:5000/swagger

# Frontend
npm install
ng serve     # http://localhost:4200

