// pages/_app.js
import { ChakraProvider } from '@chakra-ui/react'
import theme from '../theme/theme'



// 3. Pass the `theme` prop to the `ChakraProvider`
function MyApp({ Component, pageProps }:{Component:any,pageProps:any}) {
  return (
    <ChakraProvider theme={theme}>
      <Component {...pageProps} />
    </ChakraProvider>
  )
}

export default MyApp