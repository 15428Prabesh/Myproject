import { extendTheme } from "@chakra-ui/react"

const colors={
   darkBackground:{
    900:'#171923',
    800:'#1A202C',
    700:'#2D3748',
    600:'#4A5568',
    500:'#718096',
    400:'#A0AEC0'

   }
}

const theme= extendTheme({
    colors
  })

  export default theme