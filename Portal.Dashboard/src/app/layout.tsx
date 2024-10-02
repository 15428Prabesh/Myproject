// app/layout.tsx
import Footer from "./components/footer/Footer";
import { Providers } from "./provider";

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body>
        <Providers >
          {children}
          <Footer/>
        </Providers>
      </body>
    </html>
  );
}