'use client'

import {
  Flex,
  Box,
  FormControl,
  FormLabel,
  Input,
  InputGroup,
  InputRightElement,
  Stack,
  Button,
  Heading,
  Text,
  useColorModeValue,
  Link,
} from '@chakra-ui/react'
import React, {  useState } from 'react'
import { ViewIcon, ViewOffIcon } from '@chakra-ui/icons'
import {  ScriptProps } from 'next/script'
import { LOGIN } from '@/app/utils/Endpoints'
import Fetch from '@/app/utils/Fetch'


export default function LoginForm(props:ScriptProps) {
  const [showPassword, setShowPassword] = useState(false)
  const [email,setEmail]=useState('')
  const [password,setPassword]=useState('')


  const handleLogin =async()=>{

    const requestOptions={
      url:LOGIN,
      method:'POST',
      headers:{
        'Content-Type': 'application/json',
      },
      body:JSON.stringify({email,password})
    }
    const response:any=await Fetch(requestOptions)
    localStorage.setItem('Token',response.jwtToken)
    window.location.href='/dashboard'
  }


  return (
    <Flex
      minH={'100vh'}
      align={'center'}
      justify={'center'}
      bg='darkBackground.700'>
      <Stack spacing={2} mx={'auto'} maxW={'lg'} py={50} px={10}>
        <Stack align={'center'}>
          <Heading fontSize={'4xl'} textAlign={'center'} color='white'>
            Sign In
          </Heading>
        </Stack>
        <Box
          rounded={'lg'}
          bg={useColorModeValue('white', 'gray.700')}
          boxShadow={'lg'}
          p={10}>
          <Stack spacing={4}>
            <FormControl id="email" isRequired>
              <FormLabel>Email address</FormLabel>
              <Input type="email" value={email} onChange={(event:React.ChangeEvent<HTMLInputElement>)=>setEmail(event.target.value)}/>
            </FormControl>
            <FormControl id="password" isRequired>
              <FormLabel>Password</FormLabel>
              <InputGroup>
                <Input type={showPassword ? 'text' : 'password'} value={password} onChange={(event:React.ChangeEvent<HTMLInputElement>)=>setPassword(event.target.value)} />
                <InputRightElement h={'full'}>
                  <Button
                    variant={'ghost'}
                    onClick={() => setShowPassword((showPassword) => !showPassword)}>
                    {showPassword ? <ViewIcon /> : <ViewOffIcon />}
                  </Button>
                </InputRightElement>
              </InputGroup>
            </FormControl>
            <Stack spacing={10} pt={2}>
              <Button
                onClick={handleLogin}
                loadingText="Submitting"
                size="lg"
                bg={'blue.400'}
                color={'white'}
                _hover={{
                  bg: 'blue.500',
                  
                }}>
                Sign in
              </Button>
            </Stack>
          </Stack>
        </Box>
      </Stack>
    </Flex>
  )
}