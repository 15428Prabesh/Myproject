// app/providers.tsx
'use client'

import { CacheProvider } from '@chakra-ui/next-js'
import { Box, ChakraProvider, extendTheme } from '@chakra-ui/react'
import theme from './theme/theme'



export function Providers({ 
    children 
  }: { 
  children: React.ReactNode 
  }) {
  return (
    <CacheProvider>
      <ChakraProvider theme={theme}>
        <Box bg='darkBackground.800'>
          {children}
        </Box>
      </ChakraProvider>
    </CacheProvider>
  )
}