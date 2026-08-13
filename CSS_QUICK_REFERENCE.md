/* ============================================================================
   TÌM KIẾM NHANH - QUICK REFERENCE
   ============================================================================

   Sử dụng Ctrl+F để tìm các chuỗi sau trong site.css:

   ============================================================================ */

/*
   ✨ THAY ĐỔI MÀU SẮC:
   Tìm: ":root {"
   Các biến cần thay:
   - --primary-color: #3B82F6          (Màu chính)
   - --secondary-color: #10B981        (Màu phụ)
   - --danger-color: #EF4444           (Màu cảnh báo)
   - --warning-color: #F59E0B          (Màu warning)

   ===================================
*/

/*
   🖼️ THAY ĐỔI BACKGROUND HÌNH ẢNH (LOGIN PAGE):
   Tìm: ".login-background {"
   Sửa background-image url() tại dòng ~102

   Ví dụ URL:
   https://images.unsplash.com/photo-1576091160550-2173dba999ef?w=1200

   ===================================
*/

/*
   📝 THAY ĐỔI FONT CHỮ:
   Tìm: "body { font-family:"
   Sửa dòng ~52 thành font bạn muốn:
   - 'Segoe UI' (hiện tại)
   - 'Open Sans'
   - 'Roboto'
   - 'Poppins'

   ===================================
*/

/*
   ⚡ THAY ĐỔI TỐC ĐỘ ANIMATION:
   Tìm: "--transition: all 0.3s"
   Thay 0.3s thành:
   - 0.1s (siêu nhanh)
   - 0.5s (chậm hơn)
   - 1s (rất chậm)

   ===================================
*/

/*
   🎴 THAY ĐỔI SỐC GÓCO CARD:
   Tìm: "border-radius: 0.75rem"
   Các nơi:
   - .login-form-wrapper (line ~155)
   - .form-input (line ~240)
   - .card-item (line ~367)
   - .stat-card (line ~457)

   Giá trị:
   - 0.25rem (gần vuông)
   - 0.5rem (bo nhẹ)
   - 0.75rem (hiện tại)
   - 1rem (bo nhiều)

   ===================================
*/

/*
   🔲 THAY ĐỔI SHADOW (BÓNG):
   Các biến shadow:
   - --shadow-sm:  Bóng nhẹ (login form)
   - --shadow-md:  Bóng trung bình (card hover)
   - --shadow-lg:  Bóng lớn (nút button hover)
   - --shadow-xl:  Bóng rất lớn

   Tìm các dòng ~30-33 để chỉnh độ bóng

   ===================================
*/

/*
   📏 THAY ĐỔI PADDING / SPACING:
   Tìm các chỗ này:

   - .dashboard-container: padding (line ~320)
   - .login-form-container: padding (line ~161)
   - .card-item: padding (line ~387-388)
   - .cards-grid: gap (line ~360)

   Giá trị:
   - 0.5rem = 8px
   - 1rem = 16px
   - 1.5rem = 24px
   - 2rem = 32px

   ===================================
*/

/*
   🎨 THAY ĐỔI GRADIENT (MÀU DỰC):
   Tìm tất cả chỗ có:
   "background: linear-gradient(135deg, ... 0%, ... 100%)"

   Ví dụ hiện tại:
   - Login: Blue → Green
   - Card headers: Các màu khác nhau
   - Buttons: Primary color → Hover color

   135deg = hướng gradient (45 độ từ trái trên sang phải dưới)

   ===================================
*/

/*
   🔘 THAY ĐỔI NÚT BẤMCÓ MẶT:
   Login button - Tìm ".btn-login" (line ~262)
   - padding: 0.875rem 1.5rem
   - font-size: 1rem
   - border-radius: 0.5rem
   - background: linear-gradient(...)

   Hover effect - line ~271
   - transform: translateY(-2px)  (nâng lên 2px)
   - box-shadow: var(--shadow-lg)

   ===================================
*/

/*
   📱 THAY ĐỔI CHO MOBILE:
   Các @media queries:
   - @media (min-width: 768px) - Tablet trở lên
   - @media (min-width: 992px) - Desktop trở lên
   - @media (max-width: 768px) - Mobile
   - @media (max-width: 480px) - Small mobile

   Tìm từng @media để chỉnh sửa responsive

   ===================================
*/

/*
   🌈 THEME DARK MODE (TO CREATE):
   Thêm vào sau :root {}:

   @media (prefers-color-scheme: dark) {
	   :root {
		   --bg-primary: #1F2937;
		   --bg-secondary: #111827;
		   --text-primary: #F3F4F6;
		   --text-secondary: #D1D5DB;
	   }
   }

   ===================================
*/

/*
   ✅ DANH SÁCH THAY ĐỔI PHỔ BIẾN:

   Đổi màu xanh sang tím:
   1. --primary-color: #8B5CF6
   2. --primary-hover: #7C3AED
   3. --primary-light: #F3E8FF

   Đổi font sang Roboto:
   1. Thêm @import 'Roboto' từ Google Fonts
   2. Sửa font-family: 'Roboto', sans-serif

   Thêm background cho login:
   1. Tìm .login-background
   2. Thêm background-image: url(...)
   3. Thay linear-gradient màu

   Tăng size card:
   1. .card-item: padding 1.5rem → 2rem
   2. .card-title: font-size 1.3rem → 1.5rem
   3. .cards-grid: gap 2rem → 2.5rem

   ===================================
*/
