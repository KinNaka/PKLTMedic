<!-- ============================================================================
	 HƯỚNG DẪN TÙY CHỈNH GIAO DIỆN - UI CUSTOMIZATION GUIDE
	 ============================================================================

	 File này giúp bạn dễ dàng thay đổi giao diện mà không cần hiểu sâu CSS.
	 Tất cả các tùy chỉnh chính nằm trong wwwroot/css/site.css

	 ============================================================================ -->

# 📱 HƯỚNG DẪN TÙY CHỈNH GIAO DIỆN

## 🎨 1. THAY ĐỔI MÀU SẮC (COLORS)

Mở file: `wwwroot/css/site.css`

Tìm section `:root` (dòng ~18), thay đổi các giá trị màu:

```css
:root {
	/* Màu sắc chính */
	--primary-color: #3B82F6;      /* ← Thay đổi ở đây (Blue hiện tại) */
	--primary-hover: #2563EB;      /* ← Màu khi hover */
	--primary-light: #EFF6FF;      /* ← Màu sáng */

	/* Màu phụ */
	--secondary-color: #10B981;    /* ← Màu xanh lá hiện tại */
	--danger-color: #EF4444;       /* ← Màu đỏ */
	--warning-color: #F59E0B;      /* ← Màu cam */
}
```

### Các trang web để lấy mã màu:
- https://colorhexa.com/ - Để chọn màu dễ dàng
- https://coolors.co/ - Tìm bảng màu đẹp

### Ví dụ thay đổi:
- Thay đổi primary thành **Tím**: `#8B5CF6`
- Thay đổi primary thành **Đỏ**: `#EF4444`
- Thay đổi primary thành **Xanh lục**: `#22C55E`

---

## 🎭 2. THAY ĐỔI HÌNH ẢNH NỀN (LOGIN PAGE BACKGROUND)

Mở file: `wwwroot/css/site.css`

Tìm `.login-background` (dòng ~97), thay đổi background-image:

```css
.login-background {
	background-image: 
		url('https://images.unsplash.com/photo-...'),  /* ← URL ảnh của bạn */
		linear-gradient(135deg, var(--primary-color) 0%, var(--secondary-color) 100%);
}
```

### Trang web có ảnh đẹp miễn phí:
- https://unsplash.com/ - Ảnh y tế, công nghệ, chuyên nghiệp
- https://pexels.com/ - Ảnh miễn phí chất lượng cao
- https://pixabay.com/ - Ảnh y tế, tòa nhà, xanh lá

### Ví dụ URL ảnh:
```
https://images.unsplash.com/photo-1576091160550-2173dba999ef?w=1200
```

**Lưu ý**: Chọn ảnh có mối liên quan đến phòng khám/y tế

---

## 📝 3. THAY ĐỔI FONT CHỮ (TYPOGRAPHY)

Mở file: `wwwroot/css/site.css`

Tìm `body { font-family: ... }` (dòng ~52):

```css
body {
	font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
	/* ↑ Thay đổi ở đây */
}
```

### Font Google phổ biến:
- `'Open Sans', sans-serif` - Đơn giản, chuyên nghiệp
- `'Roboto', sans-serif` - Modern, sạch sẽ
- `'Poppins', sans-serif` - Trendy, dễ đọc
- `'Inter', sans-serif` - Tối ưu cho màn hình

### Cách sử dụng Google Fonts:
1. Vào https://fonts.google.com/
2. Chọn font, copy `@import` code
3. Dán vào đầu file `wwwroot/css/site.css`
4. Thay đổi `font-family`

---

## 🚀 4. THAY ĐỔI VẬN TỐC ANIMATION (ANIMATION SPEED)

Mở file: `wwwroot/css/site.css`

Tìm `--transition` trong `:root`:

```css
:root {
	--transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
	/* Thay 0.3s → 0.5s để chậm hơn, 0.1s để nhanh hơn */
}
```

---

## 🎴 5. THAY ĐỔI CARD (THẺ CHỨC NĂNG)

### Thay icon:
Mở file: `Views/Home/Index.cshtml`

Tìm các emoji như `👥`, `🧾`, `📥`, ...

Thay bằng:
- Emoji khác: https://emojipedia.org/
- Hoặc icon SVG (tìm trên https://heroicons.com/)

### Thay màu gradient card:
Tìm `style="background: linear-gradient(...)"` trong card

```html
<div class="card-header" style="background: linear-gradient(135deg, #3B82F6 0%, #2563EB 100%);">
	<!-- Thay đổi hex colors #3B82F6 và #2563EB -->
</div>
```

---

## 📐 6. THAY ĐỔI KÍCH THƯỚC (SPACING & PADDING)

Mở file: `wwwroot/css/site.css`

Các lớp có thể chỉnh sửa:
- `.dashboard-container { padding: 2rem 1rem; }` - Khoảng cách ngoài
- `.card-item { border-radius: 0.75rem; }` - Độ bo góc
- `.cards-grid { gap: 2rem; }` - Khoảng cách giữa card

Giá trị tham khảo:
- `0.5rem` = 8px
- `1rem` = 16px
- `2rem` = 32px

---

## 🔘 7. THAY ĐỔI NÚT BẤMLOGIN (LOGIN BUTTON)

Mở file: `wwwroot/css/site.css`

Tìm `.btn-login`:

```css
.btn-login {
	padding: 0.875rem 1.5rem;  /* ← Kích thước nút */
	background: linear-gradient(...);  /* ← Màu nút */
	border-radius: 0.5rem;  /* ← Độ bo góc */
	font-size: 1rem;  /* ← Kích thước chữ */
}
```

---

## 🔍 8. THAY ĐỔIĐỘ SÁNG TỐI (THEME - LIGHT/DARK)

**Để tạo theme tối:**

1. Thêm vào phía trên `:root` trong `site.css`:

```css
/* Light Theme (mặc định) */
:root {
	--primary-color: #3B82F6;
	/* ... */
}

/* Dark Theme */
@media (prefers-color-scheme: dark) {
	:root {
		--bg-primary: #1F2937;      /* Màu nền tối */
		--bg-secondary: #111827;    /* Màu card tối */
		--text-primary: #F3F4F6;    /* Chữ sáng */
		--text-secondary: #D1D5DB;  /* Chữ xám */
	}
}
```

---

## 📱 9. VÀ ĐIỀU CHỈNH TRÊN MOBILE

Mở file: `wwwroot/css/site.css`

Tìm `@media (max-width: 768px)` để chỉnh sửa giao diện mobile

```css
@media (max-width: 768px) {
	.dashboard-title {
		font-size: 2rem;  /* ← Giảm kích thước trên di động */
	}

	.cards-grid {
		grid-template-columns: 1fr;  /* ← 1 cột thay vì 3 */
		gap: 1.5rem;
	}
}
```

---

## 💾 10. LƯU NHỮNG THAY ĐỔI

Sau khi sửa file CSS:
1. Lưu file (Ctrl + S)
2. Refresh trang web (F5)
3. Xóa cache trình duyệt nếu cần (Ctrl + Shift + Delete)

---

## 🎓 TIPS THIẾT KẾ

### Chọn màu hài hòa:
- **Monochromatic**: Dùng 1 màu + các độ sáng tối khác nhau
- **Complementary**: Dùng 2 màu đối diện trên vòng tròn màu
- **Analogous**: Dùng 2-3 màu kế nhau trên vòng tròn

### Spacing tốt:
- Không quá chật: `gap: 1.5rem` trở lên
- Padding nội dung: `1.5rem` hoặc `2rem`
- Margin giữa section: `2rem` hoặc `3rem`

### Font tốt để đọc:
- Kích thước chữ: 16px (1rem) trở lên cho body
- Line-height: 1.5 hoặc 1.6
- Contrast tốt: chữ đen trên nền trắng

---

## 🆘 CẦN GIÚP ĐỠ?

Nếu thay đổi CSS bị lỗi:
1. Mở Developer Tools (F12)
2. Vào tab "Console" để xem lỗi
3. Kiểm tra syntax CSS (dấu `;` và `{}`)
4. Xóa thay đổi và thử lại

---

**Happy Customizing! 🎨✨**
